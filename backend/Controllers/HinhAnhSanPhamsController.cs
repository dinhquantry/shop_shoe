using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HinhAnhSanPhamsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HinhAnhSanPhamsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HinhAnhSanPhamAdminDto>>> GetAll([FromQuery] int? maSanPham)
        {
            var query = BaseImageQuery();

            if (maSanPham.HasValue)
            {
                query = query.Where(x => x.MaSanPham == maSanPham.Value);
            }

            var items = await query
                .OrderBy(x => x.MaSanPham)
                .ThenByDescending(x => x.IsMain)
                .ThenBy(x => x.ThuTu)
                .ToListAsync();

            return Ok(items.Select(MapImage));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<HinhAnhSanPhamAdminDto>> GetById(int id)
        {
            var item = await BaseImageQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(MapImage(item));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HinhAnhSanPhamAdminDto>> Create([FromBody] HinhAnhSanPhamRequestDto request)
        {
            if (request.ThuTu < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Thu tu anh khong hop le.",
                    Detail = "Thu tu khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var product = await _context.SanPhams.FindAsync(request.MaSanPham);
            if (product is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Ma san pham khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var hasAnyImage = await _context.HinhAnhSanPhams.AnyAsync(x => x.MaSanPham == request.MaSanPham);
            var shouldBeMain = request.IsMain || !hasAnyImage;

            if (shouldBeMain)
            {
                var currentMainImages = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == request.MaSanPham && x.IsMain)
                    .ToListAsync();

                foreach (var image in currentMainImages)
                {
                    image.IsMain = false;
                }
            }

            var entity = new HinhAnhSanPham
            {
                MaSanPham = request.MaSanPham,
                ImageUrl = request.ImageUrl.Trim(),
                IsMain = shouldBeMain,
                ThuTu = request.ThuTu
            };

            _context.HinhAnhSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            var created = await BaseImageQuery().FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapImage(created));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<HinhAnhSanPhamAdminDto>> Update(int id, [FromBody] HinhAnhSanPhamRequestDto request)
        {
            if (request.ThuTu < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Thu tu anh khong hop le.",
                    Detail = "Thu tu khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var entity = await _context.HinhAnhSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var oldProductId = entity.MaSanPham;
            var oldWasMain = entity.IsMain;
            var oldImageUrl = entity.ImageUrl;
            var product = await _context.SanPhams.FindAsync(request.MaSanPham);
            if (product is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Ma san pham khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var shouldBeMain = request.IsMain;

            if (!request.IsMain)
            {
                var hasAnotherMain = await _context.HinhAnhSanPhams.AnyAsync(x =>
                    x.MaSanPham == request.MaSanPham && x.Id != id && x.IsMain);

                if (!hasAnotherMain)
                {
                    shouldBeMain = true;
                }
            }

            if (shouldBeMain)
            {
                var currentMainImages = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == request.MaSanPham && x.Id != id && x.IsMain)
                    .ToListAsync();

                foreach (var image in currentMainImages)
                {
                    image.IsMain = false;
                }
            }

            entity.MaSanPham = request.MaSanPham;
            entity.ImageUrl = request.ImageUrl.Trim();
            entity.IsMain = shouldBeMain;
            entity.ThuTu = request.ThuTu;

            await _context.SaveChangesAsync();

            if (!string.Equals(oldImageUrl, entity.ImageUrl, StringComparison.OrdinalIgnoreCase))
            {
                await TryDeleteLocalFileIfUnused(oldImageUrl, id);
            }

            if (oldProductId != request.MaSanPham && oldWasMain)
            {
                var replacement = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == oldProductId)
                    .OrderBy(x => x.ThuTu)
                    .ThenBy(x => x.Id)
                    .FirstOrDefaultAsync();

                if (replacement is not null && !replacement.IsMain)
                {
                    replacement.IsMain = true;
                    await _context.SaveChangesAsync();
                }
            }

            var updated = await BaseImageQuery().FirstAsync(x => x.Id == id);
            return Ok(MapImage(updated));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.HinhAnhSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var productId = entity.MaSanPham;
            var wasMain = entity.IsMain;
            var imageUrl = entity.ImageUrl;

            _context.HinhAnhSanPhams.Remove(entity);
            await _context.SaveChangesAsync();

            await TryDeleteLocalFileIfUnused(imageUrl);

            if (wasMain)
            {
                var replacement = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == productId)
                    .OrderBy(x => x.ThuTu)
                    .ThenBy(x => x.Id)
                    .FirstOrDefaultAsync();

                if (replacement is not null)
                {
                    replacement.IsMain = true;
                    await _context.SaveChangesAsync();
                }
            }

            return NoContent();
        }

        private IQueryable<HinhAnhSanPham> BaseImageQuery()
        {
            return _context.HinhAnhSanPhams
                .Include(x => x.SanPham);
        }

        private async Task TryDeleteLocalFileIfUnused(string? imageUrl, int? excludingImageId = null)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var query = _context.HinhAnhSanPhams.Where(x => x.ImageUrl == imageUrl);
            if (excludingImageId.HasValue)
            {
                query = query.Where(x => x.Id != excludingImageId.Value);
            }

            var isStillUsed = await query.AnyAsync();
            if (isStillUsed)
            {
                return;
            }

            var webRootPath = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                : _environment.WebRootPath;

            var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(webRootPath, relativePath);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
        }

        private static HinhAnhSanPhamAdminDto MapImage(HinhAnhSanPham item)
        {
            return new HinhAnhSanPhamAdminDto
            {
                Id = item.Id,
                MaSanPham = item.MaSanPham,
                TenSanPham = item.SanPham?.TenSanPham ?? string.Empty,
                ImageUrl = item.ImageUrl,
                IsMain = item.IsMain,
                ThuTu = item.ThuTu
            };
        }
    }
}
