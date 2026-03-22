using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class HoaDon
    {
        public int Id { get; set; }

        public int MaNguoiDung { get; set; }

        public int? MaKhuyenMai { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoaiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DiaChiNhan { get; set; } = string.Empty;

        [StringLength(255)]
        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TamTinh { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienGiam { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PhiVanChuyen { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public byte TrangThaiDonHang { get; set; } = 0;

        public byte TrangThaiThanhToan { get; set; } = 0;

        public byte PhuongThucThanhToan { get; set; } = 0;

        public DateTime? NgayThanhToan { get; set; }

        [ForeignKey(nameof(MaNguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(MaKhuyenMai))]
        public KhuyenMai? KhuyenMai { get; set; }

        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}