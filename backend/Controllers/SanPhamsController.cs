using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SanPhamsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SanPhamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SanPhamListItemDto>>> GetAll()
        {
            var items = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .Include(x => x.ThuongHieu)
                .Include(x => x.BienTheSanPhams)
                .Include(x => x.HinhAnhSanPhams)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapSanPhamList));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<SanPhamDetailDto>> GetById(int id)
        {
            var item = await _context.SanPhams
                .Include(x => x.DanhMuc)
                .Include(x => x.ThuongHieu)
                .Include(x => x.HinhAnhSanPhams)
                .Include(x => x.BienTheSanPhams)
                    .ThenInclude(x => x.Size)
                .Include(x => x.BienTheSanPhams)
                    .ThenInclude(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapSanPhamDetail(item));
        }

        [HttpPost]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<ActionResult<SanPhamDetailDto>> Create([FromBody] SanPhamRequestDto request)
        {
            var categoryExists = await _context.DanhMucs.AnyAsync(x => x.Id == request.MaDanhMuc);
            var brandExists = await _context.ThuongHieus.AnyAsync(x => x.Id == request.MaThuongHieu);

            if (!categoryExists || !brandExists)
            {
                return BadRequest(new { message = "Danh muc hoac thuong hieu khong hop le." });
            }

            var entity = new SanPham
            {
                TenSanPham = request.TenSanPham.Trim(),
                MaDanhMuc = request.MaDanhMuc,
                MaThuongHieu = request.MaThuongHieu,
                GiaBan = request.GiaBan,
                MoTa = request.MoTa?.Trim(),
                TrangThai = request.TrangThai,
                CreatedAt = DateTime.Now
            };

            _context.SanPhams.Add(entity);
            await _context.SaveChangesAsync();

            var created = await LoadSanPhamDetail(entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapSanPhamDetail(created!));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<ActionResult<SanPhamDetailDto>> Update(int id, [FromBody] SanPhamRequestDto request)
        {
            var entity = await _context.SanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var categoryExists = await _context.DanhMucs.AnyAsync(x => x.Id == request.MaDanhMuc);
            var brandExists = await _context.ThuongHieus.AnyAsync(x => x.Id == request.MaThuongHieu);

            if (!categoryExists || !brandExists)
            {
                return BadRequest(new { message = "Danh muc hoac thuong hieu khong hop le." });
            }

            entity.TenSanPham = request.TenSanPham.Trim();
            entity.MaDanhMuc = request.MaDanhMuc;
            entity.MaThuongHieu = request.MaThuongHieu;
            entity.GiaBan = request.GiaBan;
            entity.MoTa = request.MoTa?.Trim();
            entity.TrangThai = request.TrangThai;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var updated = await LoadSanPhamDetail(id);
            return Ok(MapSanPhamDetail(updated!));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SanPhams.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var hasReferences = await _context.BienTheSanPhams.AnyAsync(x => x.MaSanPham == id)
                || await _context.DanhGias.AnyAsync(x => x.MaSanPham == id);

            if (hasReferences)
            {
                return BadRequest(new { message = "Khong the xoa san pham da phat sinh du lieu lien quan." });
            }

            _context.SanPhams.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<SanPham?> LoadSanPhamDetail(int id)
        {
            return await _context.SanPhams
                .Include(x => x.DanhMuc)
                .Include(x => x.ThuongHieu)
                .Include(x => x.HinhAnhSanPhams)
                .Include(x => x.BienTheSanPhams)
                    .ThenInclude(x => x.Size)
                .Include(x => x.BienTheSanPhams)
                    .ThenInclude(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private static SanPhamListItemDto MapSanPhamList(SanPham item)
        {
            return new SanPhamListItemDto
            {
                Id = item.Id,
                TenSanPham = item.TenSanPham,
                MaDanhMuc = item.MaDanhMuc,
                TenDanhMuc = item.DanhMuc?.TenDanhMuc ?? string.Empty,
                MaThuongHieu = item.MaThuongHieu,
                TenThuongHieu = item.ThuongHieu?.TenThuongHieu ?? string.Empty,
                GiaBan = item.GiaBan,
                MoTa = item.MoTa,
                TrangThai = item.TrangThai,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                AnhChinh = item.HinhAnhSanPhams
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.ThuTu)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault(),
                TongSoLuongTon = item.BienTheSanPhams.Sum(x => x.SoLuongTon)
            };
        }

        private static SanPhamDetailDto MapSanPhamDetail(SanPham item)
        {
            return new SanPhamDetailDto
            {
                Id = item.Id,
                TenSanPham = item.TenSanPham,
                MaDanhMuc = item.MaDanhMuc,
                TenDanhMuc = item.DanhMuc?.TenDanhMuc ?? string.Empty,
                MaThuongHieu = item.MaThuongHieu,
                TenThuongHieu = item.ThuongHieu?.TenThuongHieu ?? string.Empty,
                GiaBan = item.GiaBan,
                MoTa = item.MoTa,
                TrangThai = item.TrangThai,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                AnhChinh = item.HinhAnhSanPhams
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.ThuTu)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault(),
                TongSoLuongTon = item.BienTheSanPhams.Sum(x => x.SoLuongTon),
                BienThes = item.BienTheSanPhams
                    .OrderBy(x => x.Id)
                    .Select(x => new BienTheSanPhamDto
                    {
                        Id = x.Id,
                        MaSize = x.MaSize,
                        TenSize = x.Size?.TenSize ?? string.Empty,
                        MaMau = x.MaMau,
                        TenMau = x.MauSac?.TenMau ?? string.Empty,
                        MaHex = x.MauSac?.MaHex,
                        SKU = x.SKU,
                        SoLuongTon = x.SoLuongTon,
                        TrangThai = x.TrangThai
                    })
                    .ToList(),
                HinhAnhs = item.HinhAnhSanPhams
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.ThuTu)
                    .Select(x => new HinhAnhSanPhamDto
                    {
                        Id = x.Id,
                        ImageUrl = x.ImageUrl,
                        IsMain = x.IsMain,
                        ThuTu = x.ThuTu
                    })
                    .ToList()
            };
        }
    }
}
