using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class NguoiDung
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MatKhauHash { get; set; } = string.Empty;

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public int MaQuyen { get; set; }

        public byte TrangThai { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(MaQuyen))]
        public PhanQuyen? PhanQuyen { get; set; }

        public ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}