using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class HinhAnhSanPham
    {
        public int Id { get; set; }

        public int MaSanPham { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;

        public int ThuTu { get; set; } = 0;

        [ForeignKey(nameof(MaSanPham))]
        public SanPham? SanPham { get; set; }
    }
}