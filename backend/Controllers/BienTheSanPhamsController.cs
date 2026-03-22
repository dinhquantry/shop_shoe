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
    [Authorize(Roles = "Admin")]
    public class BienTheSanPhamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BienTheSanPhamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BienTheSanPhamAdminDto>>> GetAll([FromQuery] int? maSanPham)
        {
            var query = BaseVariantQuery();

            if (maSanPham.HasValue)
            {
                query = query.Where(x => x.MaSanPham == maSanPham.Value);
            }

            var items = await query.OrderBy(x => x.Id).ToListAsync();
            return Ok(items.Select(MapBienThe));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BienTheSanPhamAdminDto>> GetById(int id)
        {
            var item = await BaseVariantQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(MapBienThe(item));
        }

        [HttpPost]
        public async Task<ActionResult<BienTheSanPhamAdminDto>> Create([FromBody] BienTheSanPhamRequestDto request)
        {
            var validationProblem = await ValidateVariantRequest(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var sku = request.SKU.Trim().ToUpperInvariant();
            var duplicated = await _context.BienTheSanPhams.AnyAsync(x =>
                x.SKU.ToLower() == sku.ToLower() ||
                (x.MaSanPham == request.MaSanPham && x.MaSize == request.MaSize && x.MaMau == request.MaMau));

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "SKU hoac bien the san pham da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new BienTheSanPham
            {
                MaSanPham = request.MaSanPham,
                MaSize = request.MaSize,
                MaMau = request.MaMau,
                SKU = sku,
                SoLuongTon = request.SoLuongTon,
                TrangThai = request.TrangThai
            };

            _context.BienTheSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            var created = await BaseVariantQuery().FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapBienThe(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BienTheSanPhamAdminDto>> Update(int id, [FromBody] BienTheSanPhamRequestDto request)
        {
            var entity = await _context.BienTheSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var validationProblem = await ValidateVariantRequest(request);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var sku = request.SKU.Trim().ToUpperInvariant();
            var duplicated = await _context.BienTheSanPhams.AnyAsync(x =>
                x.Id != id &&
                (x.SKU.ToLower() == sku.ToLower() ||
                 (x.MaSanPham == request.MaSanPham && x.MaSize == request.MaSize && x.MaMau == request.MaMau)));

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "SKU hoac bien the san pham da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.MaSanPham = request.MaSanPham;
            entity.MaSize = request.MaSize;
            entity.MaMau = request.MaMau;
            entity.SKU = sku;
            entity.SoLuongTon = request.SoLuongTon;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();

            var updated = await BaseVariantQuery().FirstAsync(x => x.Id == id);
            return Ok(MapBienThe(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.BienTheSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var hasReferences = await _context.GioHangs.AnyAsync(x => x.MaBienThe == id)
                || await _context.ChiTietHoaDons.AnyAsync(x => x.MaBienThe == id);

            if (hasReferences)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa bien the san pham.",
                    Detail = "Bien the da phat sinh du lieu lien quan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.BienTheSanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<BienTheSanPham> BaseVariantQuery()
        {
            return _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac);
        }

        private async Task<ActionResult?> ValidateVariantRequest(BienTheSanPhamRequestDto request)
        {
            if (request.SoLuongTon < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong ton khong hop le.",
                    Detail = "So luong ton khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            var sizeExists = await _context.Sizes.AnyAsync(x => x.Id == request.MaSize);
            var colorExists = await _context.MauSacs.AnyAsync(x => x.Id == request.MaMau);

            if (!productExists || !sizeExists || !colorExists)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu bien the khong hop le.",
                    Detail = "San pham, size hoac mau sac khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return null;
        }

        private static BienTheSanPhamAdminDto MapBienThe(BienTheSanPham item)
        {
            return new BienTheSanPhamAdminDto
            {
                Id = item.Id,
                MaSanPham = item.MaSanPham,
                TenSanPham = item.SanPham?.TenSanPham ?? string.Empty,
                MaSize = item.MaSize,
                TenSize = item.Size?.TenSize ?? string.Empty,
                MaMau = item.MaMau,
                TenMau = item.MauSac?.TenMau ?? string.Empty,
                MaHex = item.MauSac?.MaHex,
                SKU = item.SKU,
                SoLuongTon = item.SoLuongTon,
                TrangThai = item.TrangThai
            };
        }
    }
}
