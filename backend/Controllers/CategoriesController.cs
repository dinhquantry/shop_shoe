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
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //lấy toàn bộ danh mục
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var danhMucDb = await _context.DanhMucs.ToListAsync();
            var danhSachDto = _mapper.Map<List<CategoryDto>>(danhMucDb);
            return Ok(danhSachDto);
        }
        //lấy chi tiết 1 danh mục
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var cat = await _context.DanhMucs.FindAsync(id);
            if (cat == null) return NotFound();
            var categoryDto = _mapper.Map<CategoryDto>(cat);
            return Ok(categoryDto);
        }
        //sửa danh muc
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryCreateDto dto)
        {
            var cat = await _context.DanhMucs.FindAsync(id);
            if (cat == null) return NotFound("Không tìm thấy danh mục này!");
            bool isDuplicate = await _context.DanhMucs.AnyAsync(c => c.TenDm == dto.TenDm && c.MaDm != id);
            if (isDuplicate) return BadRequest("Tên danh mục đã tồn tại");
            cat.TenDm = dto.TenDm;
            cat.MoTa = dto.MoTa;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        //thêm danh mục
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
        {
            //tên danh mục không được trống
            if (string.IsNullOrWhiteSpace(dto.TenDm)) return BadRequest("tên danh mục không được để trống!");
            bool isExists = await _context.DanhMucs.AnyAsync(a => a.TenDm == dto.TenDm.Trim());
            if (isExists) return BadRequest("Danh mục đã tồn tại");
            var newCategory = _mapper.Map<DanhMuc>(dto);
            newCategory.TenDm = newCategory.TenDm.Trim();

            _context.DanhMucs.Add(newCategory);
            await _context.SaveChangesAsync();

            var categoryReturnDto = _mapper.Map<CategoryDto>(newCategory);
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.MaDm }, categoryReturnDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCategory(int id)
        {
            var cat = await _context.DanhMucs.FindAsync(id);
            if (cat == null) return NotFound("không tìm thấy danh mục này");
            bool hasProduct = await _context.SanPhams.AnyAsync(p => p.MaDm == id);
            if (hasProduct) return BadRequest("Danh mục còn tồn tại sản phẩm. Không thể xóa");
            _context.DanhMucs.Remove(cat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}