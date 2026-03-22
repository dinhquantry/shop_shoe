using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class MauSac
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenMau { get; set; } = string.Empty;

        [StringLength(7)]
        public string? MaHex { get; set; }

        public ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
    }
}
