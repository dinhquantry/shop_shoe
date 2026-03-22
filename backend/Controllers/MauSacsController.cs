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
        public async Task<ActionResult<IEnumerable<MauSac>>> GetAll()
        {
            var items = await _context.MauSacs
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MauSac>> GetById(int id)
        {
            var item = await _context.MauSacs.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<MauSac>> Create([FromBody] MauSacRequestDto request)
        {
            var normalizedName = request.TenMau.Trim();
            var exists = await _context.MauSacs
                .AnyAsync(x => x.TenMau.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Ten mau da ton tai." });
            }

            var entity = new MauSac
            {
                TenMau = normalizedName,
                MaHex = request.MaHex?.Trim()
            };

            _context.MauSacs.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MauSac>> Update(int id, [FromBody] MauSacRequestDto request)
        {
            var entity = await _context.MauSacs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenMau.Trim();
            var exists = await _context.MauSacs
                .AnyAsync(x => x.Id != id && x.TenMau.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Ten mau da ton tai." });
            }

            entity.TenMau = normalizedName;
            entity.MaHex = request.MaHex?.Trim();

            await _context.SaveChangesAsync();
            return Ok(entity);
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
                return BadRequest(new { message = "Khong the xoa mau sac dang duoc su dung boi bien the san pham." });
            }

            _context.MauSacs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
