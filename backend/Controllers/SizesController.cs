using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SizesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SizesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Size>>> GetAll()
        {
            var items = await _context.Sizes
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Size>> GetById(int id)
        {
            var item = await _context.Sizes.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Size>> Create([FromBody] SizeRequestDto request)
        {
            var normalizedName = request.TenSize.Trim();
            var exists = await _context.Sizes
                .AnyAsync(x => x.TenSize.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Ten size da ton tai." });
            }

            var entity = new Size
            {
                TenSize = normalizedName
            };

            _context.Sizes.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Size>> Update(int id, [FromBody] SizeRequestDto request)
        {
            var entity = await _context.Sizes.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenSize.Trim();
            var exists = await _context.Sizes
                .AnyAsync(x => x.Id != id && x.TenSize.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Ten size da ton tai." });
            }

            entity.TenSize = normalizedName;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Sizes.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var isUsed = await _context.BienTheSanPhams.AnyAsync(x => x.MaSize == id);
            if (isUsed)
            {
                return BadRequest(new { message = "Khong the xoa size dang duoc su dung boi bien the san pham." });
            }

            _context.Sizes.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
