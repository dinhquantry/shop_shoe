using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = AppPolicies.Management)]
    public class PhanQuyensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PhanQuyensController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhanQuyenDto>>> GetAll()
        {
            var items = await _context.PhanQuyens
                .Include(x => x.NguoiDungs)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapPhanQuyen));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PhanQuyenDto>> GetById(int id)
        {
            var item = await _context.PhanQuyens
                .Include(x => x.NguoiDungs)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapPhanQuyen(item));
        }

        [HttpPost]
        public async Task<ActionResult<PhanQuyenDto>> Create([FromBody] PhanQuyenRequestDto request)
        {
            var normalizedName = request.TenQuyen.Trim();
            var exists = await _context.PhanQuyens.AnyAsync(x => x.TenQuyen.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten quyen da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new PhanQuyen
            {
                TenQuyen = normalizedName,
                MoTa = request.MoTa?.Trim()
            };

            _context.PhanQuyens.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.PhanQuyens
                .Include(x => x.NguoiDungs)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapPhanQuyen(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PhanQuyenDto>> Update(int id, [FromBody] PhanQuyenRequestDto request)
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
                return Conflict(new ProblemDetails
                {
                    Title = "Ten quyen da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.TenQuyen = normalizedName;
            entity.MoTa = request.MoTa?.Trim();

            await _context.SaveChangesAsync();

            var updated = await _context.PhanQuyens
                .Include(x => x.NguoiDungs)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapPhanQuyen(updated));
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
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa quyen.",
                    Detail = "Quyen dang duoc gan cho nguoi dung.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.PhanQuyens.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static PhanQuyenDto MapPhanQuyen(PhanQuyen item)
        {
            return new PhanQuyenDto
            {
                Id = item.Id,
                TenQuyen = item.TenQuyen,
                MoTa = item.MoTa,
                SoNguoiDung = item.NguoiDungs.Count
            };
        }
    }
}
