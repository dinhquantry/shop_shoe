using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThuongHieusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThuongHieusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThuongHieu>>> GetAll()
        {
            var items = await _context.ThuongHieus.OrderBy(x => x.Id).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ThuongHieu>> GetById(int id)
        {
            var item = await _context.ThuongHieus.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ThuongHieu>> Create([FromBody] ThuongHieu request)
        {
            var normalizedName = request.TenThuongHieu.Trim();
            var exists = await _context.ThuongHieus.AnyAsync(x => x.TenThuongHieu.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Ten thuong hieu da ton tai." });
            }

            var entity = new ThuongHieu
            {
                TenThuongHieu = normalizedName,
                LogoUrl = request.LogoUrl?.Trim(),
                MoTa = request.MoTa?.Trim(),
                TrangThai = request.TrangThai
            };

            _context.ThuongHieus.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ThuongHieu>> Update(int id, [FromBody] ThuongHieu request)
        {
            var entity = await _context.ThuongHieus.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenThuongHieu.Trim();
            var exists = await _context.ThuongHieus.AnyAsync(x => x.Id != id && x.TenThuongHieu.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Ten thuong hieu da ton tai." });
            }

            entity.TenThuongHieu = normalizedName;
            entity.LogoUrl = request.LogoUrl?.Trim();
            entity.MoTa = request.MoTa?.Trim();
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ThuongHieus.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.SanPhams.AnyAsync(x => x.MaThuongHieu == id);
            if (isUsed)
            {
                return BadRequest(new { message = "Khong the xoa thuong hieu dang duoc su dung." });
            }

            _context.ThuongHieus.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
