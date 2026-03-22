using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class ChiTietHoaDon
    {
        public int Id { get; set; }

        public int MaHoaDon { get; set; }

        public int MaBienThe { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        [ForeignKey(nameof(MaHoaDon))]
        public HoaDon? HoaDon { get; set; }

        [ForeignKey(nameof(MaBienThe))]
        public BienTheSanPham? BienTheSanPham { get; set; }

        public DanhGia? DanhGia { get; set; }
    }
}