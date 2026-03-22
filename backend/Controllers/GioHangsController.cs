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
    public class GioHangsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GioHangsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GioHangItemDto>>> GetAll([FromQuery] int? maNguoiDung)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var isManagement = User.HasManagementAccess();
            var query = _context.GioHangs
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.SanPham)
                        .ThenInclude(x => x!.HinhAnhSanPhams)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.Size)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.MauSac)
                .AsQueryable();

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

            var items = await query.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(items.Select(MapGioHangItem));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GioHangItemDto>> GetById(int id)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var item = await LoadCartItem(id);
            if (item is null)
            {
                return NotFound();
            }

            if (!User.HasManagementAccess() && item.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            return Ok(MapGioHangItem(item));
        }

        [HttpPost]
        public async Task<ActionResult<GioHangItemDto>> Create([FromBody] GioHangRequestDto request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var resolvedUserId = User.HasManagementAccess() ? request.MaNguoiDung : currentUserId.Value;
            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == resolvedUserId);
            var variant = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                    .ThenInclude(x => x!.HinhAnhSanPhams)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (!userExists || variant is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu gio hang khong hop le.",
                    Detail = "Nguoi dung hoac bien the san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.SoLuong <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong khong hop le.",
                    Detail = "So luong phai lon hon 0.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var existing = await _context.GioHangs.FirstOrDefaultAsync(x =>
                x.MaNguoiDung == resolvedUserId && x.MaBienThe == request.MaBienThe);

            if (existing is not null)
            {
                var newQuantity = existing.SoLuong + request.SoLuong;
                if (newQuantity > variant.SoLuongTon)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "So luong vuot ton kho.",
                        Detail = "So luong trong gio hang vuot qua ton kho hien tai.",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                existing.SoLuong = newQuantity;
                await _context.SaveChangesAsync();

                var updated = await LoadCartItem(existing.Id);
                return Ok(MapGioHangItem(updated!));
            }

            if (request.SoLuong > variant.SoLuongTon)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong vuot ton kho.",
                    Detail = "So luong trong gio hang vuot qua ton kho hien tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var entity = new GioHang
            {
                MaNguoiDung = resolvedUserId,
                MaBienThe = request.MaBienThe,
                SoLuong = request.SoLuong,
                CreatedAt = DateTime.Now
            };

            _context.GioHangs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await LoadCartItem(entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapGioHangItem(created!));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GioHangItemDto>> Update(int id, [FromBody] GioHangRequestDto request)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var isManagement = User.HasManagementAccess();
            var entity = await _context.GioHangs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            if (!isManagement && entity.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            var resolvedUserId = isManagement ? request.MaNguoiDung : currentUserId.Value;
            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == resolvedUserId);
            var variant = await _context.BienTheSanPhams
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (!userExists || variant is null)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Du lieu gio hang khong hop le.",
                    Detail = "Nguoi dung hoac bien the san pham khong ton tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.SoLuong <= 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong khong hop le.",
                    Detail = "So luong phai lon hon 0.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (request.SoLuong > variant.SoLuongTon)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "So luong vuot ton kho.",
                    Detail = "So luong trong gio hang vuot qua ton kho hien tai.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var duplicated = await _context.GioHangs.AnyAsync(x =>
                x.Id != id &&
                x.MaNguoiDung == resolvedUserId &&
                x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "San pham da ton tai trong gio hang.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.MaNguoiDung = resolvedUserId;
            entity.MaBienThe = request.MaBienThe;
            entity.SoLuong = request.SoLuong;

            await _context.SaveChangesAsync();

            var updated = await LoadCartItem(entity.Id);
            return Ok(MapGioHangItem(updated!));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.GetUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized();
            }

            var entity = await _context.GioHangs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            if (!User.HasManagementAccess() && entity.MaNguoiDung != currentUserId.Value)
            {
                return Forbid();
            }

            _context.GioHangs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<GioHang?> LoadCartItem(int id)
        {
            return await _context.GioHangs
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.SanPham)
                        .ThenInclude(x => x!.HinhAnhSanPhams)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.Size)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.MauSac)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private static GioHangItemDto MapGioHangItem(GioHang item)
        {
            var bienThe = item.BienTheSanPham!;
            var sanPham = bienThe.SanPham!;

            return new GioHangItemDto
            {
                Id = item.Id,
                MaNguoiDung = item.MaNguoiDung,
                MaBienThe = item.MaBienThe,
                SoLuong = item.SoLuong,
                CreatedAt = item.CreatedAt,
                TenSanPham = sanPham.TenSanPham,
                GiaBan = sanPham.GiaBan,
                SKU = bienThe.SKU,
                TenSize = bienThe.Size?.TenSize ?? string.Empty,
                TenMau = bienThe.MauSac?.TenMau ?? string.Empty,
                MaHex = bienThe.MauSac?.MaHex,
                AnhChinh = sanPham.HinhAnhSanPhams
                    .OrderByDescending(x => x.IsMain)
                    .ThenBy(x => x.ThuTu)
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault(),
                SoLuongTon = bienThe.SoLuongTon,
                ThanhTien = item.SoLuong * sanPham.GiaBan
            };
        }
    }
}
