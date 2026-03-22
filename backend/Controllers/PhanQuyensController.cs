using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhanQuyensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhanQuyensController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhanQuyen>>> GetAll()
        {
            var items = await _context.PhanQuyens.OrderBy(x => x.Id).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PhanQuyen>> GetById(int id)
        {
            var item = await _context.PhanQuyens.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<PhanQuyen>> Create([FromBody] PhanQuyen request)
        {
            var normalizedName = request.TenQuyen.Trim();
            var exists = await _context.PhanQuyens.AnyAsync(x => x.TenQuyen.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Ten quyen da ton tai." });
            }

            var entity = new PhanQuyen
            {
                TenQuyen = normalizedName,
                MoTa = request.MoTa?.Trim()
            };

            _context.PhanQuyens.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PhanQuyen>> Update(int id, [FromBody] PhanQuyen request)
        {
            var entity = await _context.PhanQuyens.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenQuyen.Trim();
            var exists = await _context.PhanQuyens.AnyAsync(x => x.Id != id && x.TenQuyen.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new { message = "Ten quyen da ton tai." });
            }

            entity.TenQuyen = normalizedName;
            entity.MoTa = request.MoTa?.Trim();

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.PhanQuyens.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.NguoiDungs.AnyAsync(x => x.MaQuyen == id);
            if (isUsed)
            {
                return BadRequest(new { message = "Khong the xoa quyen dang duoc su dung." });
            }

            _context.PhanQuyens.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
