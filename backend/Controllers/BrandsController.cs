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
    public class BrandsController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public BrandsController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // lay toan bo thuong hieu
        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brandsDb = await _context.ThuongHieus.ToListAsync();
            var brandsDto = _mapper.Map<List<BrandDto>>(brandsDb);
            return Ok(brandsDto);
        }

        // lay chi tiet 1 thuong hieu
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _context.ThuongHieus.FindAsync(id);
            if (brand == null) return NotFound("Khong tim thay thuong hieu");

            var brandDto = _mapper.Map<BrandDto>(brand);
            return Ok(brandDto);
        }

        // them thuong hieu
        [HttpPost]
        public async Task<IActionResult> CreateBrand(BrandCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenTh)) return BadRequest("Ten thuong hieu khong duoc de trong");

            var brandName = dto.TenTh.Trim();
            bool isExists = await _context.ThuongHieus.AnyAsync(b => b.TenTh == brandName);
            if (isExists) return BadRequest("Thuong hieu da ton tai");

            var newBrand = _mapper.Map<ThuongHieu>(dto);
            newBrand.TenTh = brandName;
            newBrand.MoTa = dto.MoTa?.Trim();

            _context.ThuongHieus.Add(newBrand);
            await _context.SaveChangesAsync();

            var brandReturnDto = _mapper.Map<BrandDto>(newBrand);
            return CreatedAtAction(nameof(GetBrand), new { id = newBrand.MaTh }, brandReturnDto);
        }

        // sua thuong hieu
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBrand(int id, BrandCreateDto dto)
        {
            var brand = await _context.ThuongHieus.FindAsync(id);
            if (brand == null) return NotFound("Khong tim thay thuong hieu nay!");

            if (string.IsNullOrWhiteSpace(dto.TenTh)) return BadRequest("Ten thuong hieu khong duoc de trong");

            var brandName = dto.TenTh.Trim();
            bool isDuplicate = await _context.ThuongHieus.AnyAsync(b => b.TenTh == brandName && b.MaTh != id);
            if (isDuplicate) return BadRequest("Ten thuong hieu da ton tai");

            brand.TenTh = brandName;
            brand.MoTa = dto.MoTa?.Trim();

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // xoa thuong hieu
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.ThuongHieus.FindAsync(id);
            if (brand == null) return NotFound("Khong tim thay thuong hieu nay");

            bool hasProduct = await _context.SanPhams.AnyAsync(p => p.MaTh == id);
            if (hasProduct) return BadRequest("Thuong hieu con ton tai san pham. Khong the xoa");

            _context.ThuongHieus.Remove(brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        // ==========================================
        // TẢI LÊN LOGO CHO THƯƠNG HIỆU
        // ==========================================
        [HttpPost("{id}/logo")]
        public async Task<IActionResult> UploadLogo(int id, IFormFile file) // Hứng trực tiếp IFormFile từ Form
        {
            // 1. Kiểm tra thương hiệu có tồn tại không
            var brand = await _context.ThuongHieus.FindAsync(id);
            if (brand == null) return NotFound("Không tìm thấy thương hiệu này!");

            // 2. Kiểm dịch File
            if (file == null || file.Length == 0) return BadRequest("File ảnh không được để trống!");
            if (file.Length > 5 * 1024 * 1024) return BadRequest("Dung lượng ảnh không được vượt quá 5MB.");

            // Với Logo, ta nên cho phép thêm đuôi .svg vì logo web thường dùng SVG cho nét
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension)) return BadRequest("Chỉ chấp nhận ảnh (.jpg, .png, .webp, .svg).");

            // 3. Xử lý lưu file vào ổ cứng
            var newFileName = Guid.NewGuid().ToString() + fileExtension;
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "brands"); // Thư mục brands riêng biệt

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. CẬP NHẬT ĐƯỜNG DẪN VÀO BẢNG THƯƠNG HIỆU
            brand.Logo = $"/images/brands/{newFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Upload logo thành công!", logoPath = brand.Logo });
        }
    }
}
