using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class KhuyenMai
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal PhanTramGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiamToiDa { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTriDonToiThieu { get; set; } = 0;

        public int SoLuong { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    }
}