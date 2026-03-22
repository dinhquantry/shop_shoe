using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<NguoiDung> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            AppDbContext context,
            IPasswordHasher<NguoiDung> passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("dang-ky")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> DangKy([FromBody] DangKyRequestDto request)
        {
            var duplicated = await _context.NguoiDungs.AnyAsync(x =>
                x.Email.ToLower() == request.Email.Trim().ToLower() ||
                x.TenDangNhap.ToLower() == request.TenDangNhap.Trim().ToLower() ||
                x.SoDienThoai == request.SoDienThoai.Trim());

            if (duplicated)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Thong tin dang ky da ton tai.",
                    Detail = "Email, ten dang nhap hoac so dien thoai da duoc su dung.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            var maQuyenMacDinh = await _context.PhanQuyens
                .Where(x => x.TenQuyen.ToLower() == "customer" || x.TenQuyen.ToLower() == "khachhang")
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync() ?? 1;

            var user = new NguoiDung
            {
                HoTen = request.HoTen.Trim(),
                Email = request.Email.Trim(),
                SoDienThoai = request.SoDienThoai.Trim(),
                TenDangNhap = request.TenDangNhap.Trim(),
                DiaChi = request.DiaChi?.Trim(),
                MaQuyen = maQuyenMacDinh,
                TrangThai = 1,
                CreatedAt = DateTime.Now
            };

            user.MatKhauHash = _passwordHasher.HashPassword(user, request.MatKhau);

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            user = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstAsync(x => x.Id == user.Id);

            var response = _jwtTokenService.CreateToken(user);
            return CreatedAtAction(nameof(Me), null, response);
        }

        [HttpPost("dang-nhap")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> DangNhap([FromBody] DangNhapRequestDto request)
        {
            var loginValue = request.TenDangNhapHoacEmail.Trim().ToLower();

            var user = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstOrDefaultAsync(x =>
                    x.Email.ToLower() == loginValue ||
                    x.TenDangNhap.ToLower() == loginValue);

            if (user is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Dang nhap that bai.",
                    Detail = "Thong tin dang nhap khong chinh xac.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            if (user.TrangThai == 0)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Tai khoan bi khoa.",
                    Detail = "Tai khoan hien dang bi vo hieu hoa.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.MatKhauHash, request.MatKhau);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Dang nhap that bai.",
                    Detail = "Thong tin dang nhap khong chinh xac.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            return Ok(_jwtTokenService.CreateToken(user));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<NguoiDungDto>> Me()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var user = await _context.NguoiDungs
                .Include(x => x.PhanQuyen)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(new NguoiDungDto
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
            });
        }
    }
}
