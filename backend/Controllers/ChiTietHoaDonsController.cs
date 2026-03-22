using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChiTietHoaDonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChiTietHoaDonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChiTietHoaDonAdminDto>>> GetAll([FromQuery] int? maHoaDon)
        {
            var query = BaseOrderDetailQuery();

            if (maHoaDon.HasValue)
            {
                query = query.Where(x => x.MaHoaDon == maHoaDon.Value);
            }

            var items = await query.OrderBy(x => x.Id).ToListAsync();
            return Ok(items.Select(MapChiTietHoaDon));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ChiTietHoaDonAdminDto>> GetById(int id)
        {
            var item = await BaseOrderDetailQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(MapChiTietHoaDon(item));
        }

        [HttpPost]
        public async Task<ActionResult<ChiTietHoaDonAdminDto>> Create([FromBody] ChiTietHoaDonRequestDto request)
        {
            if (request.SoLuong <= 0 || request.DonGia < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Thong tin chi tiet hoa don khong hop le.",
                    Detail = "So luong phai lon hon 0 va don gia khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var order = await _context.HoaDons
                .Include(x => x.KhuyenMai)
                .FirstOrDefaultAsync(x => x.Id == request.MaHoaDon);

            var variant = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (order is null || variant is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu chi tiet hoa don khong hop le.",
                    Detail = "Hoa don hoac bien the san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (order.TrangThaiDonHang >= 2)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the thay doi chi tiet hoa don.",
                    Detail = "Hoa don da chuyen sang giai doan giao nhan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var duplicated = await _context.ChiTietHoaDons.AnyAsync(x =>
                x.MaHoaDon == request.MaHoaDon && x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Chi tiet hoa don da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            if (request.SoLuong > variant.SoLuongTon)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong vuot qua ton kho.",
                    Detail = $"SKU {variant.SKU} chi con {variant.SoLuongTon} san pham.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var entity = new ChiTietHoaDon
            {
                MaHoaDon = request.MaHoaDon,
                MaBienThe = request.MaBienThe,
                SoLuong = request.SoLuong,
                DonGia = request.DonGia,
                ThanhTien = request.SoLuong * request.DonGia
            };

            variant.SoLuongTon -= request.SoLuong;
            _context.ChiTietHoaDons.Add(entity);

            await _context.SaveChangesAsync();
            await RecalculateOrderTotals(order.Id);
            await transaction.CommitAsync();

            var created = await BaseOrderDetailQuery().FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapChiTietHoaDon(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ChiTietHoaDonAdminDto>> Update(int id, [FromBody] ChiTietHoaDonRequestDto request)
        {
            if (request.SoLuong <= 0 || request.DonGia < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Thong tin chi tiet hoa don khong hop le.",
                    Detail = "So luong phai lon hon 0 va don gia khong duoc am.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var entity = await _context.ChiTietHoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            if (request.MaHoaDon != entity.MaHoaDon)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong ho tro doi hoa don cho chi tiet.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var order = await _context.HoaDons
                .Include(x => x.KhuyenMai)
                .FirstOrDefaultAsync(x => x.Id == entity.MaHoaDon);

            var currentVariant = await _context.BienTheSanPhams.FirstAsync(x => x.Id == entity.MaBienThe);
            var newVariant = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (order is null || newVariant is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu chi tiet hoa don khong hop le.",
                    Detail = "Hoa don hoac bien the san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (order.TrangThaiDonHang >= 2)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the thay doi chi tiet hoa don.",
                    Detail = "Hoa don da chuyen sang giai doan giao nhan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var duplicated = await _context.ChiTietHoaDons.AnyAsync(x =>
                x.Id != id &&
                x.MaHoaDon == request.MaHoaDon &&
                x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Chi tiet hoa don da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            currentVariant.SoLuongTon += entity.SoLuong;

            if (request.SoLuong > newVariant.SoLuongTon)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong vuot qua ton kho.",
                    Detail = $"SKU {newVariant.SKU} chi con {newVariant.SoLuongTon} san pham sau khi doi chi tiet.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            newVariant.SoLuongTon -= request.SoLuong;

            entity.MaBienThe = request.MaBienThe;
            entity.SoLuong = request.SoLuong;
            entity.DonGia = request.DonGia;
            entity.ThanhTien = request.SoLuong * request.DonGia;

            await _context.SaveChangesAsync();
            await RecalculateOrderTotals(order.Id);
            await transaction.CommitAsync();

            var updated = await BaseOrderDetailQuery().FirstAsync(x => x.Id == entity.Id);
            return Ok(MapChiTietHoaDon(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.ChiTietHoaDons.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var order = await _context.HoaDons
                .Include(x => x.KhuyenMai)
                .FirstOrDefaultAsync(x => x.Id == entity.MaHoaDon);

            if (order is null)
            {
                return NotFound();
            }

            if (order.TrangThaiDonHang >= 2)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa chi tiet hoa don.",
                    Detail = "Hoa don da chuyen sang giai doan giao nhan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var hasReview = await _context.DanhGias.AnyAsync(x => x.MaChiTietHoaDon == id);
            if (hasReview)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa chi tiet hoa don.",
                    Detail = "Chi tiet hoa don da co danh gia.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var variant = await _context.BienTheSanPhams.FirstAsync(x => x.Id == entity.MaBienThe);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            variant.SoLuongTon += entity.SoLuong;
            _context.ChiTietHoaDons.Remove(entity);

            await _context.SaveChangesAsync();
            await RecalculateOrderTotals(order.Id);
            await transaction.CommitAsync();

            return NoContent();
        }

        private IQueryable<ChiTietHoaDon> BaseOrderDetailQuery()
        {
            return _context.ChiTietHoaDons
                .Include(x => x.HoaDon)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.SanPham)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.Size)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.MauSac);
        }

        private async Task RecalculateOrderTotals(int orderId)
        {
            var order = await _context.HoaDons
                .Include(x => x.KhuyenMai)
                .Include(x => x.ChiTietHoaDons)
                .FirstAsync(x => x.Id == orderId);

            order.TamTinh = order.ChiTietHoaDons.Sum(x => x.ThanhTien);
            order.SoTienGiam = 0;

            if (order.KhuyenMai is not null && order.TamTinh >= order.KhuyenMai.GiaTriDonToiThieu)
            {
                var soTienGiam = Math.Round(order.TamTinh * order.KhuyenMai.PhanTramGiam / 100m, 2);
                if (order.KhuyenMai.GiamToiDa > 0 && soTienGiam > order.KhuyenMai.GiamToiDa)
                {
                    soTienGiam = order.KhuyenMai.GiamToiDa;
                }

                order.SoTienGiam = soTienGiam;
            }

            order.TongTien = Math.Max(0, order.TamTinh - order.SoTienGiam + order.PhiVanChuyen);
            await _context.SaveChangesAsync();
        }

        private static ChiTietHoaDonAdminDto MapChiTietHoaDon(ChiTietHoaDon item)
        {
            return new ChiTietHoaDonAdminDto
            {
                Id = item.Id,
                MaHoaDon = item.MaHoaDon,
                MaBienThe = item.MaBienThe,
                TenSanPham = item.BienTheSanPham?.SanPham?.TenSanPham ?? string.Empty,
                SKU = item.BienTheSanPham?.SKU ?? string.Empty,
                TenSize = item.BienTheSanPham?.Size?.TenSize ?? string.Empty,
                TenMau = item.BienTheSanPham?.MauSac?.TenMau ?? string.Empty,
                SoLuong = item.SoLuong,
                DonGia = item.DonGia,
                ThanhTien = item.ThanhTien
            };
        }
    }
}
