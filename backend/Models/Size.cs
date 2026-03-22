using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Size
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TenSize { get; set; } = string.Empty;
        public ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
    }
}