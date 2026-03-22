using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhuyenMaisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KhuyenMaisController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KhuyenMai>>> GetAll()
        {
            var items = await _context.KhuyenMais.OrderBy(x => x.Id).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<KhuyenMai>> GetById(int id)
        {
            var item = await _context.KhuyenMais.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<KhuyenMai>> Create([FromBody] KhuyenMai request)
        {
            var code = request.Code.Trim();
            var exists = await _context.KhuyenMais.AnyAsync(x => x.Code.ToLower() == code.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Code khuyen mai da ton tai." });
            }

            var entity = new KhuyenMai
            {
                Code = code,
                PhanTramGiam = request.PhanTramGiam,
                GiamToiDa = request.GiamToiDa,
                GiaTriDonToiThieu = request.GiaTriDonToiThieu,
                SoLuong = request.SoLuong,
                NgayBatDau = request.NgayBatDau,
                NgayKetThuc = request.NgayKetThuc,
                TrangThai = request.TrangThai
            };

            _context.KhuyenMais.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<KhuyenMai>> Update(int id, [FromBody] KhuyenMai request)
        {
            var entity = await _context.KhuyenMais.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var code = request.Code.Trim();
            var exists = await _context.KhuyenMais.AnyAsync(x => x.Id != id && x.Code.ToLower() == code.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Code khuyen mai da ton tai." });
            }

            entity.Code = code;
            entity.PhanTramGiam = request.PhanTramGiam;
            entity.GiamToiDa = request.GiamToiDa;
            entity.GiaTriDonToiThieu = request.GiaTriDonToiThieu;
            entity.SoLuong = request.SoLuong;
            entity.NgayBatDau = request.NgayBatDau;
            entity.NgayKetThuc = request.NgayKetThuc;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.KhuyenMais.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.HoaDons.AnyAsync(x => x.MaKhuyenMai == id);
            if (isUsed)
            {
                return BadRequest(new { message = "Khong the xoa khuyen mai da ap dung cho hoa don." });
            }

            _context.KhuyenMais.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
