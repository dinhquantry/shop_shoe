using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BienTheSanPhamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BienTheSanPhamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BienTheSanPham>>> GetAll()
        {
            var items = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BienTheSanPham>> GetById(int id)
        {
            var item = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<BienTheSanPham>> Create([FromBody] BienTheSanPham request)
        {
            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            var sizeExists = await _context.Sizes.AnyAsync(x => x.Id == request.MaSize);
            var colorExists = await _context.MauSacs.AnyAsync(x => x.Id == request.MaMau);

            if (!productExists || !sizeExists || !colorExists)
            {
                return BadRequest(new { message = "San pham, size hoac mau sac khong hop le." });
            }

            var sku = request.SKU.Trim();
            var duplicated = await _context.BienTheSanPhams.AnyAsync(x =>
                x.SKU.ToLower() == sku.ToLower() ||
                (x.MaSanPham == request.MaSanPham && x.MaSize == request.MaSize && x.MaMau == request.MaMau));

            if (duplicated)
            {
                return Conflict(new { message = "SKU hoac bien the san pham da ton tai." });
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

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BienTheSanPham>> Update(int id, [FromBody] BienTheSanPham request)
        {
            var entity = await _context.BienTheSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            var sizeExists = await _context.Sizes.AnyAsync(x => x.Id == request.MaSize);
            var colorExists = await _context.MauSacs.AnyAsync(x => x.Id == request.MaMau);

            if (!productExists || !sizeExists || !colorExists)
            {
                return BadRequest(new { message = "San pham, size hoac mau sac khong hop le." });
            }

            var sku = request.SKU.Trim();
            var duplicated = await _context.BienTheSanPhams.AnyAsync(x =>
                x.Id != id &&
                (x.SKU.ToLower() == sku.ToLower() ||
                 (x.MaSanPham == request.MaSanPham && x.MaSize == request.MaSize && x.MaMau == request.MaMau)));

            if (duplicated)
            {
                return Conflict(new { message = "SKU hoac bien the san pham da ton tai." });
            }

            entity.MaSanPham = request.MaSanPham;
            entity.MaSize = request.MaSize;
            entity.MaMau = request.MaMau;
            entity.SKU = sku;
            entity.SoLuongTon = request.SoLuongTon;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(entity);
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
                return BadRequest(new { message = "Khong the xoa bien the san pham da phat sinh du lieu lien quan." });
            }

            _context.BienTheSanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
