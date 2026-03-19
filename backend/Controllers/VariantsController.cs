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
    public class VariantsController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public VariantsController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetVariants()
        {
            var variantsDb = await _context.BienTheSanPhams
                .Include(v => v.MaSpNavigation)
                .Include(v => v.MaSizeNavigation)
                .Include(v => v.MaMauNavigation)
                .ToListAsync();

            var variantsDto = _mapper.Map<List<VariantDto>>(variantsDb);
            return Ok(variantsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVariant(int id)
        {
            var variant = await _context.BienTheSanPhams
                .Include(v => v.MaSpNavigation)
                .Include(v => v.MaSizeNavigation)
                .Include(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(v => v.MaBienThe == id);

            if (variant == null) return NotFound("Không tìm thấy biến thể này!");

            var variantDto = _mapper.Map<VariantDto>(variant);
            return Ok(variantDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVariant(VariantCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Sku)) return BadRequest("SKU không được để trống.");

            var sku = dto.Sku.Trim();

            bool checkProduct = await _context.SanPhams.AnyAsync(sp => sp.MaSp == dto.MaSp);
            if (!checkProduct) return BadRequest("Sản phẩm được chọn không tồn tại trong hệ thống!");

            bool checkSize = await _context.Sizes.AnyAsync(s => s.MaSize == dto.MaSize);
            if (!checkSize) return BadRequest("Size được chọn không tồn tại trong hệ thống!");

            bool checkColor = await _context.MauSacs.AnyAsync(m => m.MaMau == dto.MaMau);
            if (!checkColor) return BadRequest("Màu sắc được chọn không tồn tại trong hệ thống!");

            bool isDuplicateVariant = await _context.BienTheSanPhams.AnyAsync(v =>
                v.MaSp == dto.MaSp &&
                v.MaSize == dto.MaSize &&
                v.MaMau == dto.MaMau);
            if (isDuplicateVariant) return BadRequest("Biến thể với sản phẩm, size và màu này đã tồn tại.");

            bool isSkuExists = await _context.BienTheSanPhams.AnyAsync(v => v.Sku == sku);
            if (isSkuExists) return BadRequest("SKU này đã được sử dụng cho biến thể khác.");

            var newVariant = _mapper.Map<BienTheSanPham>(dto);
            newVariant.Sku = sku;

            _context.BienTheSanPhams.Add(newVariant);
            await _context.SaveChangesAsync();

            var createdVariant = await _context.BienTheSanPhams
                .Include(v => v.MaSpNavigation)
                .Include(v => v.MaSizeNavigation)
                .Include(v => v.MaMauNavigation)
                .FirstOrDefaultAsync(v => v.MaBienThe == newVariant.MaBienThe);

            var variantReturnDto = _mapper.Map<VariantDto>(createdVariant);
            return CreatedAtAction(nameof(GetVariant), new { id = newVariant.MaBienThe }, variantReturnDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVariant(int id, VariantCreateDto dto)
        {
            var variant = await _context.BienTheSanPhams.FindAsync(id);
            if (variant == null) return NotFound("Không tìm thấy biến thể này!");

            if (string.IsNullOrWhiteSpace(dto.Sku)) return BadRequest("SKU không được để trống.");

            var sku = dto.Sku.Trim();

            bool checkProduct = await _context.SanPhams.AnyAsync(sp => sp.MaSp == dto.MaSp);
            if (!checkProduct) return BadRequest("Sản phẩm được chọn không tồn tại trong hệ thống!");

            bool checkSize = await _context.Sizes.AnyAsync(s => s.MaSize == dto.MaSize);
            if (!checkSize) return BadRequest("Size được chọn không tồn tại trong hệ thống!");

            bool checkColor = await _context.MauSacs.AnyAsync(m => m.MaMau == dto.MaMau);
            if (!checkColor) return BadRequest("Màu sắc được chọn không tồn tại trong hệ thống!");

            bool isDuplicateVariant = await _context.BienTheSanPhams.AnyAsync(v =>
                v.MaSp == dto.MaSp &&
                v.MaSize == dto.MaSize &&
                v.MaMau == dto.MaMau &&
                v.MaBienThe != id);
            if (isDuplicateVariant) return BadRequest("Biến thể với sản phẩm, size và màu này đã tồn tại.");

            bool isSkuExists = await _context.BienTheSanPhams.AnyAsync(v => v.Sku == sku && v.MaBienThe != id);
            if (isSkuExists) return BadRequest("SKU này đã được sử dụng cho biến thể khác.");

            variant.MaSp = dto.MaSp;
            variant.MaSize = dto.MaSize;
            variant.MaMau = dto.MaMau;
            variant.SoLuongTon = dto.SoLuongTon;
            variant.Sku = sku;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.BienTheSanPhams.FindAsync(id);
            if (variant == null) return NotFound("Không tìm thấy biến thể này!");

            bool hasOrderDetails = await _context.ChiTietHoaDons.AnyAsync(c => c.MaBienThe == id);
            if (hasOrderDetails) return BadRequest("Không thể xóa biến thể này vì nó đang tồn tại trong chi tiết hóa đơn.");

            bool hasCartItems = await _context.GioHangs.AnyAsync(g => g.MaBienThe == id);
            if (hasCartItems) return BadRequest("Không thể xóa biến thể này vì nó đang tồn tại trong giỏ hàng.");

            _context.BienTheSanPhams.Remove(variant);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // ==========================================
        // LẤY DANH SÁCH BIẾN THỂ THEO MÃ SẢN PHẨM (CỰC KỲ QUAN TRỌNG CHO GIAO DIỆN)
        // ==========================================
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetVariantsByProduct(int productId)
        {
            // Kiểm tra xem đôi giày này có thật không
            bool checkProduct = await _context.SanPhams.AnyAsync(sp => sp.MaSp == productId);
            if (!checkProduct) return NotFound("Không tìm thấy sản phẩm này!");

            // Chỉ Lọc (Where) lấy những biến thể thuộc về productId này
            var variantsDb = await _context.BienTheSanPhams
                .Include(v => v.MaSizeNavigation)
                .Include(v => v.MaMauNavigation)
                .Include(v => v.MaSpNavigation) // Phải có dòng này để AutoMapper lấy được Tên Sản Phẩm
                .Where(v => v.MaSp == productId) 
                .ToListAsync();

            var variantsDto = _mapper.Map<List<VariantDto>>(variantsDb);
            return Ok(variantsDto);
        }
    }
}
