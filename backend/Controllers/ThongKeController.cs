using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ThongKeController : ControllerBase
    {
        private const byte TrangThaiHoanTat = 3;

        private readonly AppDbContext _context;

        public ThongKeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("tong-doanh-thu")]
        public async Task<ActionResult<TongDoanhThuDto>> GetTongDoanhThu([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var validationProblem = ValidateDateRange(tuNgay, denNgay);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var completedOrders = ApplyOrderDateRange(
                    _context.HoaDons
                        .AsNoTracking()
                        .Where(x => x.TrangThaiDonHang == TrangThaiHoanTat),
                    tuNgay,
                    denNgay);

            var tongDoanhThu = await completedOrders
                .Select(x => (decimal?)x.TongTien)
                .SumAsync() ?? 0m;

            var soDonHoanTat = await completedOrders.CountAsync();

            return Ok(new TongDoanhThuDto
            {
                TuNgay = tuNgay?.Date,
                DenNgay = denNgay?.Date,
                TongDoanhThu = tongDoanhThu,
                SoDonHoanTat = soDonHoanTat
            });
        }

        [HttpGet("so-don-theo-trang-thai")]
        public async Task<ActionResult<IEnumerable<DonHangTheoTrangThaiDto>>> GetSoDonTheoTrangThai([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var validationProblem = ValidateDateRange(tuNgay, denNgay);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var statusCounts = await ApplyOrderDateRange(
                    _context.HoaDons.AsNoTracking(),
                    tuNgay,
                    denNgay)
                .GroupBy(x => x.TrangThaiDonHang)
                .Select(group => new
                {
                    TrangThaiDonHang = group.Key,
                    SoLuongDonHang = group.Count()
                })
                .ToListAsync();

            var result = Enumerable.Range(0, 4)
                .Select(index => (byte)index)
                .Select(status => new DonHangTheoTrangThaiDto
                {
                    TrangThaiDonHang = status,
                    TenTrangThai = GetOrderStatusName(status),
                    SoLuongDonHang = statusCounts
                        .FirstOrDefault(x => x.TrangThaiDonHang == status)
                        ?.SoLuongDonHang ?? 0
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("top-san-pham-ban-chay")]
        public async Task<ActionResult<IEnumerable<TopSanPhamBanChayDto>>> GetTopSanPhamBanChay(
            [FromQuery] int top = 10,
            [FromQuery] DateTime? tuNgay = null,
            [FromQuery] DateTime? denNgay = null)
        {
            if (top <= 0 || top > 50)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Gia tri top khong hop le.",
                    Detail = "Tham so top phai nam trong khoang 1 den 50.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var validationProblem = ValidateDateRange(tuNgay, denNgay);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var topProducts = await ApplyOrderDetailDateRange(
                    _context.ChiTietHoaDons
                        .AsNoTracking()
                        .Where(x => x.HoaDon != null && x.HoaDon.TrangThaiDonHang == TrangThaiHoanTat),
                    tuNgay,
                    denNgay,
                    x => x.HoaDon!.NgayDat)
                .GroupBy(x => new
                {
                    MaSanPham = x.BienTheSanPham!.MaSanPham,
                    TenSanPham = x.BienTheSanPham!.SanPham!.TenSanPham,
                    GiaBan = x.BienTheSanPham.SanPham.GiaBan,
                    TrangThai = x.BienTheSanPham.SanPham.TrangThai
                })
                .Select(group => new
                {
                    group.Key.MaSanPham,
                    group.Key.TenSanPham,
                    group.Key.GiaBan,
                    group.Key.TrangThai,
                    TongSoLuongDaBan = group.Sum(x => x.SoLuong),
                    TongDoanhThu = group.Sum(x => x.ThanhTien),
                    SoDonHang = group.Select(x => x.MaHoaDon).Distinct().Count()
                })
                .OrderByDescending(x => x.TongSoLuongDaBan)
                .ThenByDescending(x => x.TongDoanhThu)
                .ThenBy(x => x.TenSanPham)
                .Take(top)
                .ToListAsync();

            var productIds = topProducts.Select(x => x.MaSanPham).ToList();
            var imageLookup = await LoadMainImageLookup(productIds);
            var inventoryLookup = await LoadInventoryLookup(productIds);

            var result = topProducts.Select(item => new TopSanPhamBanChayDto
                {
                    MaSanPham = item.MaSanPham,
                    TenSanPham = item.TenSanPham,
                    AnhChinh = imageLookup.GetValueOrDefault(item.MaSanPham),
                    GiaBan = item.GiaBan,
                    TrangThai = item.TrangThai,
                    TongSoLuongDaBan = item.TongSoLuongDaBan,
                    TongDoanhThu = item.TongDoanhThu,
                    SoDonHang = item.SoDonHang,
                    TongSoLuongTon = inventoryLookup.GetValueOrDefault(item.MaSanPham)
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("san-pham-sap-het-hang")]
        public async Task<ActionResult<IEnumerable<SanPhamSapHetHangDto>>> GetSanPhamSapHetHang(
            [FromQuery] int nguongTon = 10,
            [FromQuery] int limit = 20)
        {
            if (nguongTon < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nguong ton khong hop le.",
                    Detail = "Tham so nguongTon khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (limit <= 0 || limit > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Gia tri limit khong hop le.",
                    Detail = "Tham so limit phai nam trong khoang 1 den 100.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var lowStockProducts = await _context.SanPhams
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.TenSanPham,
                    x.TrangThai,
                    TongSoLuongTon = x.BienTheSanPhams
                        .Select(v => (int?)v.SoLuongTon)
                        .Sum() ?? 0,
                    SoBienThe = x.BienTheSanPhams.Count(),
                    SoBienTheSapHet = x.BienTheSanPhams.Count(v => v.SoLuongTon <= nguongTon)
                })
                .Where(x => x.TongSoLuongTon <= nguongTon)
                .OrderBy(x => x.TongSoLuongTon)
                .ThenBy(x => x.TenSanPham)
                .Take(limit)
                .ToListAsync();

            var productIds = lowStockProducts.Select(x => x.Id).ToList();
            var imageLookup = await LoadMainImageLookup(productIds);

            var result = lowStockProducts.Select(item => new SanPhamSapHetHangDto
                {
                    MaSanPham = item.Id,
                    TenSanPham = item.TenSanPham,
                    AnhChinh = imageLookup.GetValueOrDefault(item.Id),
                    TrangThai = item.TrangThai,
                    TongSoLuongTon = item.TongSoLuongTon,
                    NguongCanhBao = nguongTon,
                    SoBienThe = item.SoBienThe,
                    SoBienTheSapHet = item.SoBienTheSapHet
                })
                .ToList();

            return Ok(result);
        }

        private ActionResult? ValidateDateRange(DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay.HasValue && denNgay.HasValue && tuNgay.Value.Date > denNgay.Value.Date)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khoang thoi gian khong hop le.",
                    Detail = "Tham so tuNgay khong duoc lon hon denNgay.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return null;
        }

        private static System.Linq.Expressions.Expression<Func<TSource, bool>> BuildDatePredicate<TSource>(
            System.Linq.Expressions.Expression<Func<TSource, DateTime>> selector,
            DateTime boundary,
            bool isStart)
        {
            var parameter = selector.Parameters[0];
            var comparison = isStart
                ? System.Linq.Expressions.Expression.GreaterThanOrEqual(selector.Body, System.Linq.Expressions.Expression.Constant(boundary))
                : System.Linq.Expressions.Expression.LessThan(selector.Body, System.Linq.Expressions.Expression.Constant(boundary));

            return System.Linq.Expressions.Expression.Lambda<Func<TSource, bool>>(comparison, parameter);
        }

        private IQueryable<backend.Models.HoaDon> ApplyOrderDateRange(IQueryable<backend.Models.HoaDon> query, DateTime? tuNgay, DateTime? denNgay)
        {
            var start = tuNgay?.Date;
            var endExclusive = denNgay?.Date.AddDays(1);

            if (start.HasValue)
            {
                query = query.Where(x => x.NgayDat >= start.Value);
            }

            if (endExclusive.HasValue)
            {
                query = query.Where(x => x.NgayDat < endExclusive.Value);
            }

            return query;
        }

        private IQueryable<backend.Models.ChiTietHoaDon> ApplyOrderDetailDateRange(
            IQueryable<backend.Models.ChiTietHoaDon> query,
            DateTime? tuNgay,
            DateTime? denNgay,
            System.Linq.Expressions.Expression<Func<backend.Models.ChiTietHoaDon, DateTime>> selector)
        {
            var start = tuNgay?.Date;
            var endExclusive = denNgay?.Date.AddDays(1);

            if (start.HasValue)
            {
                query = query.Where(BuildDatePredicate(selector, start.Value, isStart: true));
            }

            if (endExclusive.HasValue)
            {
                query = query.Where(BuildDatePredicate(selector, endExclusive.Value, isStart: false));
            }

            return query;
        }

        private async Task<Dictionary<int, string?>> LoadMainImageLookup(List<int> productIds)
        {
            if (productIds.Count == 0)
            {
                return new Dictionary<int, string?>();
            }

            var images = await _context.HinhAnhSanPhams
                .AsNoTracking()
                .Where(x => productIds.Contains(x.MaSanPham))
                .Select(x => new
                {
                    x.MaSanPham,
                    x.ImageUrl,
                    x.IsMain,
                    x.ThuTu
                })
                .ToListAsync();

            return images
                .GroupBy(x => x.MaSanPham)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .OrderByDescending(x => x.IsMain)
                        .ThenBy(x => x.ThuTu)
                        .Select(x => x.ImageUrl)
                        .FirstOrDefault());
        }

        private async Task<Dictionary<int, int>> LoadInventoryLookup(List<int> productIds)
        {
            if (productIds.Count == 0)
            {
                return new Dictionary<int, int>();
            }

            return await _context.BienTheSanPhams
                .AsNoTracking()
                .Where(x => productIds.Contains(x.MaSanPham))
                .GroupBy(x => x.MaSanPham)
                .Select(group => new
                {
                    MaSanPham = group.Key,
                    TongSoLuongTon = group.Sum(x => x.SoLuongTon)
                })
                .ToDictionaryAsync(x => x.MaSanPham, x => x.TongSoLuongTon);
        }

        private static string GetOrderStatusName(byte status)
        {
            return status switch
            {
                0 => "ChoXacNhan",
                1 => "DangXuLy",
                2 => "DangGiao",
                3 => "HoanTat",
                _ => $"TrangThai{status}"
            };
        }
    }
}
