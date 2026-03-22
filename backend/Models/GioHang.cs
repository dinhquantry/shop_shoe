using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class GioHang
    {
        public int Id { get; set; }

        public int MaNguoiDung { get; set; }

        public int MaBienThe { get; set; }

        public int SoLuong { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey(nameof(MaNguoiDung))]
        public NguoiDung? NguoiDung { get; set; }

        [ForeignKey(nameof(MaBienThe))]
        public BienTheSanPham? BienTheSanPham { get; set; }
    }
}