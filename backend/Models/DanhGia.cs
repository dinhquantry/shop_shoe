
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class DanhGia
    {
        public int Id { get; set; }

        public int MaChiTietHoaDon { get; set; }

        public int MaNguoiDung { get; set; }

        public int MaSanPham { get; set; }

        public byte SoSao { get; set; }

        [StringLength(1000)]
        public string? NoiDung { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        [ForeignKey(nameof(MaChiTietHoaDon))]
        public ChiTietHoaDon? ChiTietHoaDon { get; set; }

        [ForeignKey(nameof(MaNguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(MaSanPham))]
        public SanPham? SanPham { get; set; }
    }
}