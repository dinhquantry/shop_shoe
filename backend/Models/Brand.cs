using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        // 1 Thương hiệu có nhiều Sản phẩm
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}