using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var query = _context.GioHangs
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.SanPham)
                        .ThenInclude(x => x!.HinhAnhSanPhams)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.Size)
                .Include(x => x.BienTheSanPham)
                    .ThenInclude(x => x!.MauSac)
                .AsQueryable();

            if (maNguoiDung.HasValue)
            {
                query = query.Where(x => x.MaNguoiDung == maNguoiDung.Value);
            }

            var items = await query.OrderByDescending(x => x.Id).ToListAsync();
            return Ok(items.Select(MapGioHangItem));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GioHangItemDto>> GetById(int id)
        {
            var item = await LoadCartItem(id);
            return item is null ? NotFound() : Ok(MapGioHangItem(item));
        }

        [HttpPost]
        public async Task<ActionResult<GioHangItemDto>> Create([FromBody] GioHangRequestDto request)
        {
            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == request.MaNguoiDung);
            var variant = await _context.BienTheSanPhams
                .Include(x => x.SanPham)
                    .ThenInclude(x => x!.HinhAnhSanPhams)
                .Include(x => x.Size)
                .Include(x => x.MauSac)
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (!userExists || variant is null)
            {
                return BadRequest(new { message = "Nguoi dung hoac bien the san pham khong hop le." });
            }

            if (request.SoLuong <= 0)
            {
                return BadRequest(new { message = "So luong phai lon hon 0." });
            }

            var existing = await _context.GioHangs.FirstOrDefaultAsync(x =>
                x.MaNguoiDung == request.MaNguoiDung && x.MaBienThe == request.MaBienThe);

            if (existing is not null)
            {
                var newQuantity = existing.SoLuong + request.SoLuong;
                if (newQuantity > variant.SoLuongTon)
                {
                    return BadRequest(new { message = "So luong vuot qua ton kho hien tai." });
                }

                existing.SoLuong = newQuantity;
                await _context.SaveChangesAsync();

                var updated = await LoadCartItem(existing.Id);
                return Ok(MapGioHangItem(updated!));
            }

            if (request.SoLuong > variant.SoLuongTon)
            {
                return BadRequest(new { message = "So luong vuot qua ton kho hien tai." });
            }

            var entity = new GioHang
            {
                MaNguoiDung = request.MaNguoiDung,
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
            var entity = await _context.GioHangs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == request.MaNguoiDung);
            var variant = await _context.BienTheSanPhams
                .FirstOrDefaultAsync(x => x.Id == request.MaBienThe);

            if (!userExists || variant is null)
            {
                return BadRequest(new { message = "Nguoi dung hoac bien the san pham khong hop le." });
            }

            if (request.SoLuong <= 0)
            {
                return BadRequest(new { message = "So luong phai lon hon 0." });
            }

            if (request.SoLuong > variant.SoLuongTon)
            {
                return BadRequest(new { message = "So luong vuot qua ton kho hien tai." });
            }

            var duplicated = await _context.GioHangs.AnyAsync(x =>
                x.Id != id &&
                x.MaNguoiDung == request.MaNguoiDung &&
                x.MaBienThe == request.MaBienThe);

            if (duplicated)
            {
                return Conflict(new { message = "San pham da ton tai trong gio hang." });
            }

            entity.MaNguoiDung = request.MaNguoiDung;
            entity.MaBienThe = request.MaBienThe;
            entity.SoLuong = request.SoLuong;

            await _context.SaveChangesAsync();

            var updated = await LoadCartItem(entity.Id);
            return Ok(MapGioHangItem(updated!));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.GioHangs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
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
