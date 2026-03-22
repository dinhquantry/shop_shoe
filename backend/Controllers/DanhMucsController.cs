using backend.Data;
using backend.DTOs;
using backend.Models;
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
        public async Task<ActionResult<IEnumerable<DanhMuc>>> GetAll()
        {
            var items = await _context.DanhMucs
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DanhMuc>> GetById(int id)
        {
            var item = await _context.DanhMucs.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<DanhMuc>> Create([FromBody] DanhMucRequestDto request)
        {
            var normalizedName = request.TenDanhMuc.Trim();

            var exists = await _context.DanhMucs
                .AnyAsync(x => x.TenDanhMuc.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Ten danh muc da ton tai." });
            }

            var entity = new DanhMuc
            {
                TenDanhMuc = normalizedName,
                MoTa = request.MoTa?.Trim(),
                TrangThai = request.TrangThai
            };

            _context.DanhMucs.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DanhMuc>> Update(int id, [FromBody] DanhMucRequestDto request)
        {
            var entity = await _context.DanhMucs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var normalizedName = request.TenDanhMuc.Trim();
            var exists = await _context.DanhMucs
                .AnyAsync(x => x.Id != id && x.TenDanhMuc.ToLower() == normalizedName.ToLower());

            if (exists)
            {
                return Conflict(new { message = "Tên danh mục đã tồn tại." });
            }

            entity.TenDanhMuc = normalizedName;
            entity.MoTa = request.MoTa?.Trim();
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id}")]
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
                return BadRequest(new { message = "Không thể xóa danh mục này." });
            }

            _context.DanhMucs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
