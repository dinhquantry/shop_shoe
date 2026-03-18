using AutoMapper;
using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;
namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.DanhMucs
                .OrderBy(c => c.TenDm)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.DanhMucs.FindAsync(id);

            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục này.");
            }

            return Ok(_mapper.Map<CategoryDto>(category));
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryCreateDto categoryDto)
        {
            if (await _context.DanhMucs.AnyAsync(c => c.TenDm == categoryDto.Name.Trim()))
            {
                return BadRequest("Tên danh mục này đã tồn tại.");
            }

            var category = _mapper.Map<DanhMuc>(categoryDto);

            _context.DanhMucs.Add(category);
            await _context.SaveChangesAsync();

            var newCategoryDto = _mapper.Map<CategoryDto>(category);

            return CreatedAtAction(nameof(GetCategory), new { id = category.MaDm }, newCategoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục để cập nhật.");
            }

            if (await _context.DanhMucs.AnyAsync(c => c.TenDm == categoryDto.Name.Trim() && c.MaDm != id))
            {
                return BadRequest("Tên danh mục này đã tồn tại.");
            }

            _mapper.Map(categoryDto, category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi hệ thống khi cập nhật dữ liệu.");
            }

            return NoContent(); // HTTP 204: Cập nhật thành công, không cần trả về body
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var hasProducts = await _context.SanPhams.AnyAsync(p => p.MaDm == id);

            if (hasProducts)
            {
                return BadRequest("Không thể xóa danh mục đang được sử dụng.");
            }

            try
            {
                _context.DanhMucs.Remove(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Không thể xóa danh mục vì còn dữ liệu liên quan.");
            }

            return NoContent();
        }
    }
}
