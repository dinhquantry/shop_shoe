using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Lấy danh sách sản phẩm 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category) // JOIN bảng Category
                .Include(p => p.Brand)    // JOIN bảng Brand
                .Where(p => p.IsDelete == false) // Chỉ lấy các sản phẩm chưa bị ẩn
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        // Lấy chi tiết 1 sản phẩm
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.IsDelete) return NotFound("Sản phẩm không tồn tại");

            return Ok(_mapper.Map<ProductDto>(product));
        }

        // Thêm sản phẩm mới
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto productDto)
        {
            // Kiểm tra xem Category và Brand có tồn tại trong DB không
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == productDto.BrandId);

            if (!categoryExists) return BadRequest("Danh mục không hợp lệ.");
            if (!brandExists) return BadRequest("Thương hiệu không hợp lệ.");

            var product = _mapper.Map<Product>(productDto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Để trả về DTO đầy đủ tên, ta cần load lại dữ liệu từ DB
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Brand).LoadAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, _mapper.Map<ProductDto>(product));
        }
        // 4. CẬP NHẬT SẢN PHẨM
        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDelete)
            {
                return NotFound("Không tìm thấy sản phẩm hoặc sản phẩm đã bị xóa.");
            }

            // Kiểm tra xem Category và Brand mới (nếu có đổi) có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == productDto.CategoryId);
            var brandExists = await _context.Brands.AnyAsync(b => b.Id == productDto.BrandId);

            if (!categoryExists) return BadRequest("Danh mục không hợp lệ.");
            if (!brandExists) return BadRequest("Thương hiệu không hợp lệ.");

            // Ghi đè dữ liệu mới từ DTO vào Product hiện tại
            _mapper.Map(productDto, product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi hệ thống khi cập nhật dữ liệu.");
            }

            return NoContent(); // HTTP 204: Thành công
        }

        // 5. XÓA MỀM SẢN PHẨM
        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null || product.IsDelete)
            {
                return NotFound("Sản phẩm không tồn tại hoặc đã bị xóa trước đó.");
            }

            product.IsDelete = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}