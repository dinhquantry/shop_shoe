using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MauSacsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MauSacsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MauSacDto>>> GetAll()
        {
            var items = await _context.MauSacs
                .Include(x => x.BienTheSanPhams)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapMauSac));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MauSacDto>> GetById(int id)
        {
            var item = await _context.MauSacs
                .Include(x => x.BienTheSanPhams)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapMauSac(item));
        }

        [HttpPost]
        public async Task<ActionResult<MauSacDto>> Create([FromBody] MauSacRequestDto request)
        {
            var normalizedName = request.TenMau.Trim();
            var exists = await _context.MauSacs.AnyAsync(x => x.TenMau.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten mau da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new MauSac
            {
                TenMau = normalizedName,
                MaHex = request.MaHex?.Trim()
            };

            _context.MauSacs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.MauSacs
                .Include(x => x.BienTheSanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapMauSac(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MauSacDto>> Update(int id, [FromBody] MauSacRequestDto request)
        {
            var entity = await _context.MauSacs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenMau.Trim();
            var exists = await _context.MauSacs.AnyAsync(x => x.Id != id && x.TenMau.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten mau da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.TenMau = normalizedName;
            entity.MaHex = request.MaHex?.Trim();

            await _context.SaveChangesAsync();

            var updated = await _context.MauSacs
                .Include(x => x.BienTheSanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapMauSac(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.MauSacs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.BienTheSanPhams.AnyAsync(x => x.MaMau == id);
            if (isUsed)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa mau sac.",
                    Detail = "Mau sac dang duoc su dung boi bien the san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.MauSacs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static MauSacDto MapMauSac(MauSac item)
        {
            return new MauSacDto
            {
                Id = item.Id,
                TenMau = item.TenMau,
                MaHex = item.MaHex,
                SoBienThe = item.BienTheSanPhams.Count
            };
        }
    }
}
