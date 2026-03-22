using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
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
        public async Task<IActionResult> UploadProductImages([FromForm] UploadAnhSanPhamRequestDto request)
        {
            var sanPhamExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            if (!sanPhamExists)
            {
                return NotFound(new { message = "Khong tim thay san pham." });
            }

            if (request.AnhChinhIndex.HasValue &&
                (request.AnhChinhIndex.Value < 0 || request.AnhChinhIndex.Value >= request.Files.Count))
            {
                return BadRequest(new { message = "AnhChinhIndex khong hop le." });
            }

            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

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

            for (var i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                if (file.Length <= 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName);
                if (!AllowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = $"File '{file.FileName}' khong dung dinh dang anh hop le." });
                }

                var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
                var physicalPath = Path.Combine(uploadFolder, uniqueFileName);

                await using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var isMain = request.AnhChinhIndex.HasValue
                    ? request.AnhChinhIndex.Value == i
                    : !hasMainImage && uploadedImages.Count == 0;

                var image = new HinhAnhSanPham
                {
                    MaSanPham = request.MaSanPham,
                    ImageUrl = $"/uploads/products/{request.MaSanPham}/{uniqueFileName}",
                    IsMain = isMain,
                    ThuTu = nextOrder++
                };

                uploadedImages.Add(image);
            }

            if (uploadedImages.Count == 0)
            {
                return BadRequest(new { message = "Khong co file anh hop le de upload." });
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

            return Ok(uploadedImages);
        }
    }
}
