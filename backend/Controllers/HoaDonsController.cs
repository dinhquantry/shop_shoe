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
    [Authorize]
    public class HoaDonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HoaDonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDonDto>>> GetAll([FromQuery] int? maNguoiDung)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var isManagement = User.HasManagementAccess();
            var query = BaseOrderQuery();

            if (isManagement)
            {
                if (maNguoiDung.HasValue)
                {
                    query = query.Where(x => x.MaNguoiDung == maNguoiDung.Value);
                }
            }
            else
            {
                query = query.Where(x => x.MaNguoiDung == currentUserId.Value);
            }

            var items = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapHoaDon));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<HoaDonDto>> GetById(int id)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var item = await BaseOrderQuery().FirstOrDefaultAsync(x => x.Id == id);
            if (item is null)
            {
                return NotFound();
            }

            if (!User.HasManagementAccess() && item.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            return Ok(MapHoaDon(item));
        }

        [HttpPost]
        public async Task<ActionResult<HoaDonDto>> Create([FromBody] HoaDonCreateRequestDto request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var resolvedUserId = User.HasManagementAccess() ? request.MaNguoiDung : currentUserId.Value;
            if (request.Items.Count == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Hoa don khong hop le.",
                    Detail = "Hoa don phai co it nhat 1 san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var user = await _context.NguoiDungs.FindAsync(resolvedUserId);
            if (user is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nguoi dung khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var variantIds = request.Items.Select(x => x.MaBienThe).Distinct().ToList();
            var variants = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .Where(x => variantIds.Contains(x.Id))
                .ToListAsync();

            if (variants.Count != variantIds.Count)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu san pham khong hop le.",
                    Detail = "Co bien the san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var duplicatedVariant = request.Items
                .GroupBy(x => x.MaBienThe)
                .FirstOrDefault(x => x.Count() > 1);

            if (duplicatedVariant is not null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Danh sach san pham khong hop le.",
                    Detail = "Danh sach san pham khong duoc trung bien the.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            foreach (var item in request.Items)
            {
                if (item.SoLuong <= 0)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "So luong mua khong hop le.",
                        Detail = "So luong mua phai lon hon 0.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var variant = variants.First(x => x.Id == item.MaBienThe);
                if (!variant.TrangThai || variant.SanPham is null || !variant.SanPham.TrangThai)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Bien the san pham khong kha dung.",
                        Detail = $"Bien the {variant.SKU} hien khong kha dung.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (item.SoLuong > variant.SoLuongTon)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "So luong mua vuot ton kho.",
                        Detail = $"So luong mua vuot ton kho cua SKU {variant.SKU}.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            }

            var tamTinh = request.Items.Sum(item =>
            {
                var variant = variants.First(x => x.Id == item.MaBienThe);
                return item.SoLuong * variant.SanPham!.GiaBan;
            });

            KhuyenMai? khuyenMai = null;
            decimal soTienGiam = 0;

            if (request.MaKhuyenMai.HasValue)
            {
                khuyenMai = await _context.KhuyenMais.FindAsync(request.MaKhuyenMai.Value);
                if (khuyenMai is null || !khuyenMai.TrangThai)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Khuyen mai khong hop le.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var now = DateTime.Now;
                if (khuyenMai.SoLuong <= 0 || khuyenMai.NgayBatDau > now || khuyenMai.NgayKetThuc < now)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Khuyen mai hien khong ap dung duoc.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (tamTinh < khuyenMai.GiaTriDonToiThieu)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Khong du dieu kien ap khuyen mai.",
                        Detail = "Don hang chua du gia tri toi thieu de ap khuyen mai.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                soTienGiam = Math.Round(tamTinh * khuyenMai.PhanTramGiam / 100m, 2);
                if (khuyenMai.GiamToiDa > 0 && soTienGiam > khuyenMai.GiamToiDa)
                {
                    soTienGiam = khuyenMai.GiamToiDa;
                }
            }

            var tongTien = tamTinh - soTienGiam + request.PhiVanChuyen;
            if (tongTien < 0)
            {
                tongTien = 0;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var hoaDon = new HoaDon
            {
                MaNguoiDung = resolvedUserId,
                MaKhuyenMai = request.MaKhuyenMai,
                NgayDat = DateTime.Now,
                TenNguoiNhan = request.TenNguoiNhan.Trim(),
                SoDienThoaiNhan = request.SoDienThoaiNhan.Trim(),
                DiaChiNhan = request.DiaChiNhan.Trim(),
                GhiChu = request.GhiChu?.Trim(),
                TamTinh = tamTinh,
                SoTienGiam = soTienGiam,
                PhiVanChuyen = request.PhiVanChuyen,
                TongTien = tongTien,
                TrangThaiDonHang = 0,
                TrangThaiThanhToan = 0,
                PhuongThucThanhToan = request.PhuongThucThanhToan
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            foreach (var item in request.Items)
            {
                var variant = variants.First(x => x.Id == item.MaBienThe);
                var donGia = variant.SanPham!.GiaBan;

                _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                {
                    MaHoaDon = hoaDon.Id,
                    MaBienThe = item.MaBienThe,
                    SoLuong = item.SoLuong,
                    DonGia = donGia,
                    ThanhTien = donGia * item.SoLuong
                });

                variant.SoLuongTon -= item.SoLuong;
            }

            if (khuyenMai is not null)
            {
                khuyenMai.SoLuong -= 1;
            }

            var cartItems = await _context.GioHangs
                .Where(x => x.MaNguoiDung == resolvedUserId && variantIds.Contains(x.MaBienThe))
                .ToListAsync();

            if (cartItems.Count > 0)
            {
                _context.GioHangs.RemoveRange(cartItems);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var created = await BaseOrderQuery().FirstAsync(x => x.Id == hoaDon.Id);
            return CreatedAtAction(nameof(GetById), new { id = hoaDon.Id }, MapHoaDon(created));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<ActionResult<HoaDonDto>> Update(int id, [FromBody] HoaDonUpdateRequestDto request)
        {
            var entity = await _context.HoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            entity.TenNguoiNhan = request.TenNguoiNhan.Trim();
            entity.SoDienThoaiNhan = request.SoDienThoaiNhan.Trim();
            entity.DiaChiNhan = request.DiaChiNhan.Trim();
            entity.GhiChu = request.GhiChu?.Trim();
            entity.PhiVanChuyen = request.PhiVanChuyen;
            entity.TrangThaiDonHang = request.TrangThaiDonHang;
            entity.TrangThaiThanhToan = request.TrangThaiThanhToan;
            entity.PhuongThucThanhToan = request.PhuongThucThanhToan;
            entity.NgayThanhToan = request.NgayThanhToan;
            entity.TongTien = entity.TamTinh - entity.SoTienGiam + entity.PhiVanChuyen;

            await _context.SaveChangesAsync();

            var updated = await BaseOrderQuery().FirstAsync(x => x.Id == id);
            return Ok(MapHoaDon(updated));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = AppPolicies.Management)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.HoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            _context.HoaDons.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<HoaDon> BaseOrderQuery()
        {
            return _context.HoaDons
                .Include(x => x.NguoiDung)
                .Include(x => x.KhuyenMai)
                .Include(x => x.ChiTietHoaDons)
                    .ThenInclude(x => x.BienTheSanPham)
                        .ThenInclude(x => x!.SanPham)
                .Include(x => x.ChiTietHoaDons)
                    .ThenInclude(x => x.BienTheSanPham)
                        .ThenInclude(x => x!.Size)
                .Include(x => x.ChiTietHoaDons)
                    .ThenInclude(x => x.BienTheSanPham)
                        .ThenInclude(x => x!.MauSac);
        }

        private static HoaDonDto MapHoaDon(HoaDon item)
        {
            return new HoaDonDto
            {
                Id = item.Id,
                MaNguoiDung = item.MaNguoiDung,
                TenNguoiDung = item.NguoiDung?.HoTen ?? string.Empty,
                MaKhuyenMai = item.MaKhuyenMai,
                CodeKhuyenMai = item.KhuyenMai?.Code,
                NgayDat = item.NgayDat,
                TenNguoiNhan = item.TenNguoiNhan,
                SoDienThoaiNhan = item.SoDienThoaiNhan,
                DiaChiNhan = item.DiaChiNhan,
                GhiChu = item.GhiChu,
                TamTinh = item.TamTinh,
                SoTienGiam = item.SoTienGiam,
                PhiVanChuyen = item.PhiVanChuyen,
                TongTien = item.TongTien,
                TrangThaiDonHang = item.TrangThaiDonHang,
                TrangThaiThanhToan = item.TrangThaiThanhToan,
                PhuongThucThanhToan = item.PhuongThucThanhToan,
                NgayThanhToan = item.NgayThanhToan,
                ChiTietHoaDons = item.ChiTietHoaDons
                    .OrderBy(x => x.Id)
                    .Select(x => new HoaDonChiTietDto
                    {
                        Id = x.Id,
                        MaBienThe = x.MaBienThe,
                        TenSanPham = x.BienTheSanPham?.SanPham?.TenSanPham ?? string.Empty,
                        SKU = x.BienTheSanPham?.SKU ?? string.Empty,
                        TenSize = x.BienTheSanPham?.Size?.TenSize ?? string.Empty,
                        TenMau = x.BienTheSanPham?.MauSac?.TenMau ?? string.Empty,
                        SoLuong = x.SoLuong,
                        DonGia = x.DonGia,
                        ThanhTien = x.ThanhTien
                    })
                    .ToList()
            };
        }
    }
}
