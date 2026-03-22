using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DanhGiasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DanhGiasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DanhGiaDto>>> GetAll([FromQuery] int? maSanPham, [FromQuery] int? maNguoiDung)
        {
            var query = BaseReviewQuery();

            if (maSanPham.HasValue)
            {
                query = query.Where(x => x.MaSanPham == maSanPham.Value);
            }

            if (maNguoiDung.HasValue)
            {
                query = query.Where(x => x.MaNguoiDung == maNguoiDung.Value);
            }

            var items = await query.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(items.Select(MapDanhGia));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<DanhGiaDto>> GetById(int id)
        {
            var item = await BaseReviewQuery().FirstOrDefaultAsync(x => x.Id == id);
            return item is null ? NotFound() : Ok(MapDanhGia(item));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<DanhGiaDto>> Create([FromBody] DanhGiaRequestDto request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var resolvedUserId = User.IsAdmin() ? request.MaNguoiDung : currentUserId.Value;
            var validationProblem = await ValidateReviewRequest(request, resolvedUserId);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var duplicated = await _context.DanhGias.AnyAsync(x => x.MaChiTietHoaDon == request.MaChiTietHoaDon);
            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Chi tiet hoa don nay da duoc danh gia.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new DanhGia
            {
                MaChiTietHoaDon = request.MaChiTietHoaDon,
                MaNguoiDung = resolvedUserId,
                MaSanPham = request.MaSanPham,
                SoSao = request.SoSao,
                NoiDung = request.NoiDung?.Trim(),
                NgayDanhGia = request.NgayDanhGia ?? DateTime.Now,
                TrangThai = request.TrangThai
            };

            _context.DanhGias.Add(entity);
            await _context.SaveChangesAsync();

            var created = await BaseReviewQuery().FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapDanhGia(created));
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<DanhGiaDto>> Update(int id, [FromBody] DanhGiaRequestDto request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var entity = await _context.DanhGias.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            if (!User.IsAdmin() && entity.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            var resolvedUserId = User.IsAdmin() ? request.MaNguoiDung : currentUserId.Value;
            var validationProblem = await ValidateReviewRequest(request, resolvedUserId);
            if (validationProblem is not null)
            {
                return validationProblem;
            }

            var duplicated = await _context.DanhGias.AnyAsync(x =>
                x.Id != id && x.MaChiTietHoaDon == request.MaChiTietHoaDon);

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Chi tiet hoa don nay da duoc danh gia.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.MaChiTietHoaDon = request.MaChiTietHoaDon;
            entity.MaNguoiDung = resolvedUserId;
            entity.MaSanPham = request.MaSanPham;
            entity.SoSao = request.SoSao;
            entity.NoiDung = request.NoiDung?.Trim();
            entity.NgayDanhGia = request.NgayDanhGia ?? entity.NgayDanhGia;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();

            var updated = await BaseReviewQuery().FirstAsync(x => x.Id == entity.Id);
            return Ok(MapDanhGia(updated));
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var entity = await _context.DanhGias.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            if (!User.IsAdmin() && entity.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            _context.DanhGias.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<DanhGia> BaseReviewQuery()
        {
            return _context.DanhGias
                .Include(x => x.NguoiDung)
                .Include(x => x.SanPham)
                .Include(x => x.ChiTietHoaDon);
        }

        private async Task<ActionResult?> ValidateReviewRequest(DanhGiaRequestDto request, int resolvedUserId)
        {
            if (request.SoSao < 1 || request.SoSao > 5)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So sao khong hop le.",
                    Detail = "So sao phai nam trong khoang 1 den 5.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var orderDetail = await _context.ChiTietHoaDons
                .Include(x => x.HoaDon)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.SanPham)
                .FirstOrDefaultAsync(x => x.Id == request.MaChiTietHoaDon);

            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == resolvedUserId);
            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);

            if (orderDetail is null || !userExists || !productExists)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu danh gia khong hop le.",
                    Detail = "Chi tiet hoa don, nguoi dung hoac san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (orderDetail.HoaDon?.MaNguoiDung != resolvedUserId)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Nguoi dung khong khop don hang.",
                    Detail = "Chi duoc danh gia don hang cua chinh minh.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (orderDetail.HoaDon.TrangThaiDonHang != 3)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Chua the danh gia don hang nay.",
                    Detail = "Chi co the danh gia khi don hang da hoan tat.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var productIdFromDetail = orderDetail.BienTheSanPham?.SanPham?.Id;
            if (productIdFromDetail != request.MaSanPham)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "San pham danh gia khong khop.",
                    Detail = "San pham phai trung voi san pham trong chi tiet hoa don.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return null;
        }

        private static DanhGiaDto MapDanhGia(DanhGia item)
        {
            return new DanhGiaDto
            {
                Id = item.Id,
                MaChiTietHoaDon = item.MaChiTietHoaDon,
                MaNguoiDung = item.MaNguoiDung,
                TenNguoiDung = item.NguoiDung?.HoTen ?? string.Empty,
                MaSanPham = item.MaSanPham,
                TenSanPham = item.SanPham?.TenSanPham ?? string.Empty,
                SoSao = item.SoSao,
                NoiDung = item.NoiDung,
                NgayDanhGia = item.NgayDanhGia,
                TrangThai = item.TrangThai
            };
        }
    }
}
