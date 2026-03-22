using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhMucsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DanhMucsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DanhMucDto>>> GetAll()
        {
            var items = await _context.DanhMucs
                .Include(x => x.SanPhams)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapDanhMuc));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<DanhMucDto>> GetById(int id)
        {
            var item = await _context.DanhMucs
                .Include(x => x.SanPhams)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapDanhMuc(item));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DanhMucDto>> Create([FromBody] DanhMucRequestDto request)
        {
            var normalizedName = request.TenDanhMuc.Trim();
            var exists = await _context.DanhMucs.AnyAsync(x => x.TenDanhMuc.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten danh muc da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new DanhMuc
            {
                TenDanhMuc = normalizedName,
                MoTa = request.MoTa?.Trim(),
                TrangThai = request.TrangThai
            };

            _context.DanhMucs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.DanhMucs
                .Include(x => x.SanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapDanhMuc(created));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DanhMucDto>> Update(int id, [FromBody] DanhMucRequestDto request)
        {
            var entity = await _context.DanhMucs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenDanhMuc.Trim();
            var exists = await _context.DanhMucs.AnyAsync(x => x.Id != id && x.TenDanhMuc.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten danh muc da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.TenDanhMuc = normalizedName;
            entity.MoTa = request.MoTa?.Trim();
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();

            var updated = await _context.DanhMucs
                .Include(x => x.SanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapDanhMuc(updated));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.DanhMucs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var hasProducts = await _context.SanPhams.AnyAsync(x => x.MaDanhMuc == id);
            if (hasProducts)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa danh muc.",
                    Detail = "Danh muc dang duoc su dung boi san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.DanhMucs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static DanhMucDto MapDanhMuc(DanhMuc item)
        {
            return new DanhMucDto
            {
                Id = item.Id,
                TenDanhMuc = item.TenDanhMuc,
                MoTa = item.MoTa,
                TrangThai = item.TrangThai,
                SoSanPham = item.SanPhams.Count
            };
        }
    }
}
