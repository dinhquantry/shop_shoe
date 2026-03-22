using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class BienTheSanPham
    {
        public int Id { get; set; }

        public int MaSanPham { get; set; }

        public int MaSize { get; set; }

        public int MaMau { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        public int SoLuongTon { get; set; } = 0;

        public bool TrangThai { get; set; } = true;

        [ForeignKey(nameof(MaSanPham))]
        public SanPham? SanPham { get; set; }

        [ForeignKey(nameof(MaSize))]
        public Size? Size { get; set; }

        [ForeignKey(nameof(MaMau))]
        public MauSac? MauSac { get; set; }

        public ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}