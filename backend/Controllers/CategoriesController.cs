using AutoMapper;
using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 1. LẤY DANH SÁCH TẤT CẢ DANH MỤC
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            // Dùng AutoMapper biến danh sách Model thành DTO
            return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
        }

        // 2. LẤY 1 DANH MỤC THEO ID
        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục này.");
            }

            return Ok(_mapper.Map<CategoryDto>(category));
        }

        // 3. THÊM MỚI DANH MỤC
        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryCreateDto categoryDto)
        {
            // Kiểm tra trùng Slug 
            if (await _context.Categories.AnyAsync(c => c.Slug == categoryDto.Slug))
            {
                return BadRequest("Đường dẫn (Slug) này đã tồn tại.");
            }

            var category = _mapper.Map<Category>(categoryDto);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var newCategoryDto = _mapper.Map<CategoryDto>(category);

            // Trả về HTTP 201 Created và kèm theo dữ liệu vừa tạo
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, newCategoryDto);
        }

        // 4. CẬP NHẬT DANH MỤC
        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục để cập nhật.");
            }

            // Ghi đè dữ liệu mới từ DTO vào Category hiện tại
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

        // 5. XÓA DANH MỤC
        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Xóa thật khỏi DB
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}