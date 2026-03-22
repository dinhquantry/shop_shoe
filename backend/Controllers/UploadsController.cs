using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AppPolicies.Management)]
    public class UploadsController : ControllerBase
    {
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private const int MaxFileCount = 10;

        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp", ".gif"
        };

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UploadsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost("san-pham-anh")]
        [RequestSizeLimit(50_000_000)]
        public async Task<ActionResult<IEnumerable<HinhAnhSanPhamAdminDto>>> UploadProductImages([FromForm] UploadAnhSanPhamRequestDto request)
        {
            if (request.ThuTuBatDau < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Thu tu bat dau khong hop le.",
                    Detail = "Thu tu bat dau khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.Files.Count > MaxFileCount)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong file qua gioi han.",
                    Detail = $"Chi duoc upload toi da {MaxFileCount} file moi lan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var product = await _context.SanPhams.FindAsync(request.MaSanPham);
            if (product is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Khong tim thay san pham.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            if (request.AnhChinhIndex.HasValue &&
                (request.AnhChinhIndex.Value < 0 || request.AnhChinhIndex.Value >= request.Files.Count))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Anh chinh index khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var webRootPath = GetWebRootPath();
            var uploadFolder = Path.Combine(webRootPath, "uploads", "products", request.MaSanPham.ToString());
            Directory.CreateDirectory(uploadFolder);

            var existingImages = await _context.HinhAnhSanPhams
                .Where(x => x.MaSanPham == request.MaSanPham)
                .ToListAsync();

            var hasMainImage = existingImages.Any(x => x.IsMain);
            var nextOrder = existingImages.Any()
                ? Math.Max(existingImages.Max(x => x.ThuTu) + 1, request.ThuTuBatDau)
                : Math.Max(0, request.ThuTuBatDau);

            var uploadedImages = new List<HinhAnhSanPham>();
            var savedPhysicalPaths = new List<string>();

            try
            {
                for (var i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    if (file.Length <= 0)
                    {
                        continue;
                    }

                    if (file.Length > MaxFileSizeBytes)
                    {
                        CleanupFiles(savedPhysicalPaths);
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Kich thuoc file qua lon.",
                            Detail = $"File '{file.FileName}' vuot gioi han {MaxFileSizeBytes / (1024 * 1024)}MB.",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    var extension = Path.GetExtension(file.FileName);
                    if (!AllowedExtensions.Contains(extension))
                    {
                        CleanupFiles(savedPhysicalPaths);
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Dinh dang file khong hop le.",
                            Detail = $"File '{file.FileName}' khong phai dinh dang anh duoc ho tro.",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                    var physicalPath = Path.Combine(uploadFolder, uniqueFileName);

                    await using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedPhysicalPaths.Add(physicalPath);

                    var isMain = request.AnhChinhIndex.HasValue
                        ? request.AnhChinhIndex.Value == i
                        : !hasMainImage && uploadedImages.Count == 0;

                    uploadedImages.Add(new HinhAnhSanPham
                    {
                        MaSanPham = request.MaSanPham,
                        ImageUrl = $"/uploads/products/{request.MaSanPham}/{uniqueFileName}",
                        IsMain = isMain,
                        ThuTu = nextOrder++
                    });
                }

                if (uploadedImages.Count == 0)
                {
                    CleanupFiles(savedPhysicalPaths);
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Khong co file anh hop le de upload.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (uploadedImages.Any(x => x.IsMain))
                {
                    foreach (var image in existingImages.Where(x => x.IsMain))
                    {
                        image.IsMain = false;
                    }
                }

                _context.HinhAnhSanPhams.AddRange(uploadedImages);
                await _context.SaveChangesAsync();

                return Ok(uploadedImages.Select(x => new HinhAnhSanPhamAdminDto
                {
                    Id = x.Id,
                    MaSanPham = x.MaSanPham,
                    TenSanPham = product.TenSanPham,
                    ImageUrl = x.ImageUrl,
                    IsMain = x.IsMain,
                    ThuTu = x.ThuTu
                }));
            }
            catch
            {
                CleanupFiles(savedPhysicalPaths);
                throw;
            }
        }

        private string GetWebRootPath()
        {
            return string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;
        }

        private static void CleanupFiles(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
        }
    }
}
