using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ThuongHieu
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TenThuongHieu { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LogoUrl { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}