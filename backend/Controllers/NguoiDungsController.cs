using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class NguoiDungsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<NguoiDung> _passwordHasher;

        public NguoiDungsController(AppDbContext context, IPasswordHasher<NguoiDung> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDungDto>>> GetAll()
        {
            var items = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(items.Select(MapNguoiDung));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NguoiDungDto>> GetById(int id)
        {
            var item = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstOrDefaultAsync(x => x.Id == id);

            return item is null ? NotFound() : Ok(MapNguoiDung(item));
        }

        [HttpPost]
        public async Task<ActionResult<NguoiDungDto>> Create([FromBody] NguoiDungCreateRequestDto request)
        {
            var roleExists = await _context.PhanQuyens.AnyAsync(x => x.Id == request.MaQuyen);
            if (!roleExists)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Ma quyen khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var email = request.Email.Trim();
            var username = request.TenDangNhap.Trim();
            var phone = request.SoDienThoai.Trim();

            var duplicated = await _context.NguoiDungs.AnyAsync(x =>
                x.Email.ToLower() == email.ToLower() ||
                x.TenDangNhap.ToLower() == username.ToLower() ||
                x.SoDienThoai == phone);

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Thong tin nguoi dung da ton tai.",
                    Detail = "Email, ten dang nhap hoac so dien thoai da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var entity = new NguoiDung
            {
                HoTen = request.HoTen.Trim(),
                Email = email,
                SoDienThoai = phone,
                TenDangNhap = username,
                DiaChi = request.DiaChi?.Trim(),
                MaQuyen = request.MaQuyen,
                TrangThai = request.TrangThai,
                CreatedAt = DateTime.Now
            };

            entity.MatKhauHash = _passwordHasher.HashPassword(entity, request.MatKhau);

            _context.NguoiDungs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapNguoiDung(created));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<NguoiDungDto>> Update(int id, [FromBody] NguoiDungUpdateRequestDto request)
        {
            var entity = await _context.NguoiDungs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var roleExists = await _context.PhanQuyens.AnyAsync(x => x.Id == request.MaQuyen);
            if (!roleExists)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Ma quyen khong hop le.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var email = request.Email.Trim();
            var username = request.TenDangNhap.Trim();
            var phone = request.SoDienThoai.Trim();

            var duplicated = await _context.NguoiDungs.AnyAsync(x =>
                x.Id != id &&
                (x.Email.ToLower() == email.ToLower() ||
                 x.TenDangNhap.ToLower() == username.ToLower() ||
                 x.SoDienThoai == phone));

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Thong tin nguoi dung da ton tai.",
                    Detail = "Email, ten dang nhap hoac so dien thoai da ton tai.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            entity.HoTen = request.HoTen.Trim();
            entity.Email = email;
            entity.SoDienThoai = phone;
            entity.TenDangNhap = username;
            entity.DiaChi = request.DiaChi?.Trim();
            entity.MaQuyen = request.MaQuyen;
            entity.TrangThai = request.TrangThai;
            entity.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(request.MatKhauMoi))
            {
                entity.MatKhauHash = _passwordHasher.HashPassword(entity, request.MatKhauMoi);
            }

            await _context.SaveChangesAsync();

            var updated = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstAsync(x => x.Id == entity.Id);

            return Ok(MapNguoiDung(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.NguoiDungs.FindAsync(id);
            if (entity is null)
            {
                return NotFound();
            }

            var hasReferences = await _context.HoaDons.AnyAsync(x => x.MaNguoiDung == id)
                || await _context.GioHangs.AnyAsync(x => x.MaNguoiDung == id)
                || await _context.DanhGias.AnyAsync(x => x.MaNguoiDung == id);

            if (hasReferences)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Khong the xoa nguoi dung.",
                    Detail = "Nguoi dung da phat sinh du lieu lien quan.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _context.NguoiDungs.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static NguoiDungDto MapNguoiDung(NguoiDung user)
        {
            return new NguoiDungDto
            {
                Id = user.Id,
                HoTen = user.HoTen,
                Email = user.Email,
                SoDienThoai = user.SoDienThoai,
                TenDangNhap = user.TenDangNhap,
                DiaChi = user.DiaChi,
                TrangThai = user.TrangThai,
                MaQuyen = user.MaQuyen,
                TenQuyen = user.PhanQuyen?.TenQuyen ?? string.Empty,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
