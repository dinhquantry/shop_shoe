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
    public class ProductsController : ControllerBase
    {
        private readonly ShoeShopDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ShoeShopDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private static string? ValidateProductInput(ProductCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TenSp)) return "Tên sản phẩm không được để trống";
            if (dto.DonGia <= 0) return "Giá bán phải lớn hơn 0.";
            if (dto.GiaKhuyenMai.HasValue && dto.GiaKhuyenMai.Value < 0) return "Giá khuyến mãi không được nhỏ hơn 0.";
            if (dto.GiaKhuyenMai.HasValue && dto.GiaKhuyenMai.Value > dto.DonGia) return "Giá khuyến mãi không được lớn hơn giá bán.";

            return null;
        }

        //lay toan bo san pham
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var sanphams = await _context.SanPhams
            .Include(sp => sp.MaDmNavigation)
            .Include(th => th.MaThNavigation)
            .ToListAsync();
            var sanphamsDto = _mapper.Map<List<ProductDto>>(sanphams);
            return Ok(sanphamsDto);
        }
        //lấy 1 sản phẩm
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var pro = await _context.SanPhams
            .Include(sp => sp.MaDmNavigation)
            .Include(th => th.MaThNavigation)
            .FirstOrDefaultAsync(p => p.MaSp == id);

            if (pro == null) return NotFound("Không tìm thấy sản phẩm");
            var productDto = _mapper.Map<ProductDto>(pro);
            return Ok(productDto);
        }
        //thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto dto)
        {
            var validationMessage = ValidateProductInput(dto);
            if (validationMessage != null) return BadRequest(validationMessage);

            var productName = dto.TenSp.Trim();

            bool isExists = await _context.SanPhams.AnyAsync(a => a.TenSp == productName);
            if (isExists) return BadRequest("Sản phẩm đã tồn tại");

            bool checkCategory = await _context.DanhMucs.AnyAsync(a => a.MaDm == dto.MaDm);
            if (!checkCategory) return BadRequest("Danh mục này không tồn tại trong hệ thống!");
            bool checkBrand = await _context.ThuongHieus.AnyAsync(t => t.MaTh == dto.MaTh);
            if (!checkBrand) return BadRequest("Thương hiệu được chọn không tồn tại trong hệ thống!");

            var newProduct = _mapper.Map<SanPham>(dto);
            newProduct.TenSp = productName;
            newProduct.MoTa = dto.MoTa?.Trim();
            newProduct.CreatedAt = DateTime.UtcNow;

            _context.SanPhams.Add(newProduct);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .Include(p => p.MaThNavigation)
                .FirstOrDefaultAsync(p => p.MaSp == newProduct.MaSp);

            var productReturnDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.MaSp }, productReturnDto);
        }
        //sửa sản phẩm
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductCreateDto dto) // BẮT BUỘC SỬ DỤNG ProductCreateDto
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm này!");

            var validationMessage = ValidateProductInput(dto);
            if (validationMessage != null) return BadRequest(validationMessage);

            var productName = dto.TenSp.Trim();

            bool isDuplicate = await _context.SanPhams.AnyAsync(c => c.TenSp == productName && c.MaSp != id);
            if (isDuplicate) return BadRequest("Tên sản phẩm đã tồn tại");

            bool checkCategory = await _context.DanhMucs.AnyAsync(a => a.MaDm == dto.MaDm);
            if (!checkCategory) return BadRequest("Danh mục này không tồn tại trong hệ thống!");

            bool checkBrand = await _context.ThuongHieus.AnyAsync(t => t.MaTh == dto.MaTh);
            if (!checkBrand) return BadRequest("Thương hiệu được chọn không tồn tại trong hệ thống!");

            product.TenSp = productName;
            product.MoTa = dto.MoTa?.Trim();
            product.DonGia = dto.DonGia;
            product.GiaKhuyenMai = dto.GiaKhuyenMai;
            product.MaDm = dto.MaDm;
            product.MaTh = dto.MaTh;

            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        //xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteProduct(int id)
        {
            var prodct = await _context.SanPhams.FindAsync(id);
            if (prodct == null) return NotFound("không tìm thấy sản phẩm này");
            bool hasVariants = await _context.BienTheSanPhams.AnyAsync(v => v.MaSp == id);
            if (hasVariants) return BadRequest("Không thể xóa đôi giày này vì nó đang chứa các biến thể (size, màu)!");

            _context.SanPhams.Remove(prodct);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //upload ảnh
        [HttpPost("{id}/images")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] ImageUploadDto dto)
        {
            // 1. KIỂM TRA SẢN PHẨM CÓ TỒN TẠI KHÔNG
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm để thêm ảnh.");

            // 2. KIỂM DỊCH FILE TẢI LÊN
            if (dto.File == null || dto.File.Length == 0) return BadRequest("File ảnh không được để trống.");

            // Chặn dung lượng (> 5MB là cút)
            if (dto.File.Length > 5 * 1024 * 1024) return BadRequest("Dung lượng ảnh không được vượt quá 5MB.");

            // Chặn định dạng lạ (Chỉ cho phép jpg, jpeg, png, webp)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(dto.File.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension)) return BadRequest("Chỉ chấp nhận định dạng ảnh hợp lệ (.jpg, .png, .webp).");

            // 3. XỬ LÝ LƯU FILE VÀO Ổ CỨNG SERVER
            // Tạo tên file mới hoàn toàn để tránh trùng lặp (Dùng Guid - mã ngẫu nhiên)
            var newFileName = Guid.NewGuid().ToString() + fileExtension;

            // Đường dẫn vật lý trên ổ cứng (Lưu vào thư mục wwwroot/images/products)
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath); // Nếu chưa có thư mục thì tự tạo

            var filePath = Path.Combine(folderPath, newFileName);

            // Copy file từ bộ nhớ tạm vào ổ cứng
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // 4. LƯU ĐƯỜNG DẪN VÀO DATABASE
            var productImage = new HinhAnhSanPham
            {
                MaSp = id,
                DuongDan = $"/images/products/{newFileName}", // Link này Next.js sẽ dùng để hiển thị
                AnhChinh = dto.IsMain ? (byte)1 : (byte)0
            };

            _context.HinhAnhSanPhams.Add(productImage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Upload ảnh thành công!", path = productImage.DuongDan });
        }
    }
}
