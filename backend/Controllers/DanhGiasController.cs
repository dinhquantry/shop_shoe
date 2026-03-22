using backend.Data;
using backend.Models;
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
        public async Task<ActionResult<IEnumerable<DanhGia>>> GetAll()
        {
            var items = await _context.DanhGias
                .Include(x => x.NguoiDung)
                .Include(x => x.SanPham)
                .Include(x => x.ChiTietHoaDon)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DanhGia>> GetById(int id)
        {
            var item = await _context.DanhGias
                .Include(x => x.NguoiDung)
                .Include(x => x.SanPham)
                .Include(x => x.ChiTietHoaDon)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<DanhGia>> Create([FromBody] DanhGia request)
        {
            var orderDetailExists = await _context.ChiTietHoaDons.AnyAsync(x => x.Id == request.MaChiTietHoaDon);
            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == request.MaNguoiDung);
            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);

            if (!orderDetailExists || !userExists || !productExists)
            {
                return BadRequest(new { message = "Chi tiet hoa don, nguoi dung hoac san pham khong hop le." });
            }

            var duplicated = await _context.DanhGias.AnyAsync(x => x.MaChiTietHoaDon == request.MaChiTietHoaDon);
            if (duplicated)
            {
                return Conflict(new { message = "Chi tiet hoa don nay da duoc danh gia." });
            }

            var entity = new DanhGia
            {
                MaChiTietHoaDon = request.MaChiTietHoaDon,
                MaNguoiDung = request.MaNguoiDung,
                MaSanPham = request.MaSanPham,
                SoSao = request.SoSao,
                NoiDung = request.NoiDung?.Trim(),
                NgayDanhGia = request.NgayDanhGia == default ? DateTime.Now : request.NgayDanhGia,
                TrangThai = request.TrangThai
            };

            _context.DanhGias.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DanhGia>> Update(int id, [FromBody] DanhGia request)
        {
            var entity = await _context.DanhGias.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var orderDetailExists = await _context.ChiTietHoaDons.AnyAsync(x => x.Id == request.MaChiTietHoaDon);
            var userExists = await _context.NguoiDungs.AnyAsync(x => x.Id == request.MaNguoiDung);
            var productExists = await _context.SanPhams.AnyAsync(x => x.Id == request.MaSanPham);

            if (!orderDetailExists || !userExists || !productExists)
            {
                return BadRequest(new { message = "Chi tiet hoa don, nguoi dung hoac san pham khong hop le." });
            }

            var duplicated = await _context.DanhGias.AnyAsync(x =>
                x.Id != id && x.MaChiTietHoaDon == request.MaChiTietHoaDon);

            if (duplicated)
            {
                return Conflict(new { message = "Chi tiet hoa don nay da duoc danh gia." });
            }

            entity.MaChiTietHoaDon = request.MaChiTietHoaDon;
            entity.MaNguoiDung = request.MaNguoiDung;
            entity.MaSanPham = request.MaSanPham;
            entity.SoSao = request.SoSao;
            entity.NoiDung = request.NoiDung?.Trim();
            entity.NgayDanhGia = request.NgayDanhGia;
            entity.TrangThai = request.TrangThai;

            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.DanhGias.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            _context.DanhGias.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
