using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class DangKyRequestDto
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string MatKhau { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DiaChi { get; set; }
    }

    public class DangNhapRequestDto
    {
        [Required]
        public string TenDangNhapHoacEmail { get; set; } = string.Empty;

        [Required]
        public string MatKhau { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public NguoiDungDto NguoiDung { get; set; } = new();
    }

    public class NguoiDungDto
    {
        public int Id { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TenDangNhap { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public byte TrangThai { get; set; }
        public int MaQuyen { get; set; }
        public string TenQuyen { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class NguoiDungCreateRequestDto
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string MatKhau { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public int MaQuyen { get; set; }

        public byte TrangThai { get; set; } = 1;
    }

    public class NguoiDungUpdateRequestDto
    {
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6)]
        public string? MatKhauMoi { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public int MaQuyen { get; set; }

        public byte TrangThai { get; set; }
    }
}
