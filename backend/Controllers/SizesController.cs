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
        public async Task<ActionResult<IEnumerable<SizeDto>>> GetAll()
        {
            var items = await _context.Sizes
                .Include(x => x.BienTheSanPhams)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapSize));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SizeDto>> GetById(int id)
        {
            var item = await _context.Sizes
                .Include(x => x.BienTheSanPhams)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapSize(item));
        }

        [HttpPost]
        public async Task<ActionResult<SizeDto>> Create([FromBody] SizeRequestDto request)
        {
            var normalizedName = request.TenSize.Trim();
            var exists = await _context.Sizes.AnyAsync(x => x.TenSize.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten size da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new Size
            {
                TenSize = normalizedName
            };

            _context.Sizes.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.Sizes
                .Include(x => x.BienTheSanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapSize(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SizeDto>> Update(int id, [FromBody] SizeRequestDto request)
        {
            var entity = await _context.Sizes.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenSize.Trim();
            var exists = await _context.Sizes.AnyAsync(x => x.Id != id && x.TenSize.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten size da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.TenSize = normalizedName;

            await _context.SaveChangesAsync();

            var updated = await _context.Sizes
                .Include(x => x.BienTheSanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapSize(updated));
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
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa size.",
                    Detail = "Size dang duoc su dung boi bien the san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.Sizes.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static SizeDto MapSize(Size item)
        {
            return new SizeDto
            {
                Id = item.Id,
                TenSize = item.TenSize,
                SoBienThe = item.BienTheSanPhams.Count
            };
        }
    }
}
