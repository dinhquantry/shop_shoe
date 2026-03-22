using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class SanPham
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        public int MaDanhMuc { get; set; }

        public int MaThuongHieu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaBan { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(MaDanhMuc))]
        public DanhMuc? DanhMuc { get; set; }

        [ForeignKey(nameof(MaThuongHieu))]
        public ThuongHieu? ThuongHieu { get; set; }

        public ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
        public ICollection<HinhAnhSanPham> HinhAnhSanPhams { get; set; } = new List<HinhAnhSanPham>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}