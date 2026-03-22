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
    public class ThuongHieusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ThuongHieusController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ThuongHieuDto>>> GetAll()
        {
            var items = await _context.ThuongHieus
                .Include(x => x.SanPhams)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapThuongHieu));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ThuongHieuDto>> GetById(int id)
        {
            var item = await _context.ThuongHieus
                .Include(x => x.SanPhams)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapThuongHieu(item));
        }

        [HttpPost]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<ActionResult<ThuongHieuDto>> Create([FromBody] ThuongHieuRequestDto request)
        {
            var normalizedName = request.TenThuongHieu.Trim();
            var exists = await _context.ThuongHieus.AnyAsync(x => x.TenThuongHieu.ToLower() == normalizedName.ToLower());
            if (exists)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Ten thuong hieu da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
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

            var created = await _context.ThuongHieus
                .Include(x => x.SanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapThuongHieu(created));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<ActionResult<ThuongHieuDto>> Update(int id, [FromBody] ThuongHieuRequestDto request)
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
                return Conflict(new ProblemDetails
                {
                    Title = "Ten thuong hieu da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.TenThuongHieu = normalizedName;
            entity.LogoUrl = request.LogoUrl?.Trim();
            entity.MoTa = request.MoTa?.Trim();
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();

            var updated = await _context.ThuongHieus
                .Include(x => x.SanPhams)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapThuongHieu(updated));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
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
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa thuong hieu.",
                    Detail = "Thuong hieu dang duoc su dung boi san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.ThuongHieus.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ThuongHieuDto MapThuongHieu(ThuongHieu item)
        {
            return new ThuongHieuDto
            {
                Id = item.Id,
                TenThuongHieu = item.TenThuongHieu,
                LogoUrl = item.LogoUrl,
                MoTa = item.MoTa,
                TrangThai = item.TrangThai,
                SoSanPham = item.SanPhams.Count
            };
        }
    }
}
