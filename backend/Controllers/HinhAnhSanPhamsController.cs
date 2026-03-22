using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HinhAnhSanPhamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HinhAnhSanPhamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HinhAnhSanPham>>> GetAll()
        {
            var items = await _context.HinhAnhSanPhams
                .Include(x => x.SanPham)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<HinhAnhSanPham>> GetById(int id)
        {
            var item = await _context.HinhAnhSanPhams
                .Include(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<HinhAnhSanPham>> Create([FromBody] HinhAnhSanPham request)
        {
            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            if (!productExists)
            {
                return BadRequest(new { message = "Ma san pham khong hop le." });
            }

            if (request.IsMain)
            {
                var currentMainImages = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == request.MaSanPham && x.IsMain)
                    .ToListAsync();

                foreach (var image in currentMainImages)
                {
                    image.IsMain = false;
                }
            }

            var entity = new HinhAnhSanPham
            {
                MaSanPham = request.MaSanPham,
                ImageUrl = request.ImageUrl.Trim(),
                IsMain = request.IsMain,
                ThuTu = request.ThuTu
            };

            _context.HinhAnhSanPhams.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<HinhAnhSanPham>> Update(int id, [FromBody] HinhAnhSanPham request)
        {
            var entity = await _context.HinhAnhSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);
            if (!productExists)
            {
                return BadRequest(new { message = "Ma san pham khong hop le." });
            }

            if (request.IsMain)
            {
                var currentMainImages = await _context.HinhAnhSanPhams
                    .Where(x => x.MaSanPham == request.MaSanPham && x.Id != id && x.IsMain)
                    .ToListAsync();

                foreach (var image in currentMainImages)
                {
                    image.IsMain = false;
                }
            }

            entity.MaSanPham = request.MaSanPham;
            entity.ImageUrl = request.ImageUrl.Trim();
            entity.IsMain = request.IsMain;
            entity.ThuTu = request.ThuTu;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.HinhAnhSanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            _context.HinhAnhSanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
