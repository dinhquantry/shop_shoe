using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .Include(p => p.MaThNavigation)
                .Where(p => !p.Active.HasValue || p.Active.Value != 0)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .Include(p => p.MaThNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == id);

            if (product == null || product.Active == 0)
            {
                return NotFound("Sản phẩm không tồn tại");
            }

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto productDto)
        {
            if (productDto.CategoryId.HasValue)
            {
                var categoryExists = await _context.DanhMucs.AnyAsync(c => c.MaDm == productDto.CategoryId.Value);
                if (!categoryExists)
                {
                    return BadRequest("Danh mục không hợp lệ.");
                }
            }

            if (productDto.BrandId.HasValue)
            {
                var brandExists = await _context.ThuongHieus.AnyAsync(b => b.MaTh == productDto.BrandId.Value);
                if (!brandExists)
                {
                    return BadRequest("Thương hiệu không hợp lệ.");
                }
            }

            var product = _mapper.Map<SanPham>(productDto);

            _context.SanPhams.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.MaDmNavigation).LoadAsync();
            await _context.Entry(product).Reference(p => p.MaThNavigation).LoadAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.MaSp }, _mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto productDto)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null || product.Active == 0)
            {
                return NotFound("Không tìm thấy sản phẩm hoặc sản phẩm đã bị xóa.");
            }

            if (productDto.CategoryId.HasValue)
            {
                var categoryExists = await _context.DanhMucs.AnyAsync(c => c.MaDm == productDto.CategoryId.Value);
                if (!categoryExists)
                {
                    return BadRequest("Danh mục không hợp lệ.");
                }
            }

            if (productDto.BrandId.HasValue)
            {
                var brandExists = await _context.ThuongHieus.AnyAsync(b => b.MaTh == productDto.BrandId.Value);
                if (!brandExists)
                {
                    return BadRequest("Thương hiệu không hợp lệ.");
                }
            }

            _mapper.Map(productDto, product);
            product.UpdatedAt = DateTime.Now;

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);

            if (product == null || product.Active == 0)
            {
                return NotFound("Sản phẩm không tồn tại hoặc đã bị xóa trước đó.");
            }

            product.Active = 0;
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
