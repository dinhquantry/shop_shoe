using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietHoaDonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChiTietHoaDonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietHoaDon>>> GetAll()
        {
            var items = await _context.ChiTietHoaDons
                .Include(x => x.HoaDon)
                .Include(x => x.BienTheSanPham)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ChiTietHoaDon>> GetById(int id)
        {
            var item = await _context.ChiTietHoaDons
                .Include(x => x.HoaDon)
                .Include(x => x.BienTheSanPham)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ChiTietHoaDon>> Create([FromBody] ChiTietHoaDon request)
        {
            var orderExists = await _context.HoaDons.AnyAsync(x => x.Id == request.MaHoaDon);
            var variantExists = await _context.BienTheSanPhams.AnyAsync(x => x.Id == request.MaBienThe);

            if (!orderExists || !variantExists)
            {
                return BadRequest(new { message = "Hoa don hoac bien the san pham khong hop le." });
            }

            var duplicated = await _context.ChiTietHoaDons.AnyAsync(x =>
                x.MaHoaDon == request.MaHoaDon && x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new { message = "Chi tiet hoa don da ton tai." });
            }

            var entity = new ChiTietHoaDon
            {
                MaHoaDon = request.MaHoaDon,
                MaBienThe = request.MaBienThe,
                SoLuong = request.SoLuong,
                DonGia = request.DonGia,
                ThanhTien = request.ThanhTien
            };

            _context.ChiTietHoaDons.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ChiTietHoaDon>> Update(int id, [FromBody] ChiTietHoaDon request)
        {
            var entity = await _context.ChiTietHoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var orderExists = await _context.HoaDons.AnyAsync(x => x.Id == request.MaHoaDon);
            var variantExists = await _context.BienTheSanPhams.AnyAsync(x => x.Id == request.MaBienThe);

            if (!orderExists || !variantExists)
            {
                return BadRequest(new { message = "Hoa don hoac bien the san pham khong hop le." });
            }

            var duplicated = await _context.ChiTietHoaDons.AnyAsync(x =>
                x.Id != id &&
                x.MaHoaDon == request.MaHoaDon &&
                x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new { message = "Chi tiet hoa don da ton tai." });
            }

            entity.MaHoaDon = request.MaHoaDon;
            entity.MaBienThe = request.MaBienThe;
            entity.SoLuong = request.SoLuong;
            entity.DonGia = request.DonGia;
            entity.ThanhTien = request.ThanhTien;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ChiTietHoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var hasReview = await _context.DanhGias.AnyAsync(x => x.MaChiTietHoaDon == id);
            if (hasReview)
            {
                return BadRequest(new { message = "Khong the xoa chi tiet hoa don da co danh gia." });
            }

            _context.ChiTietHoaDons.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
