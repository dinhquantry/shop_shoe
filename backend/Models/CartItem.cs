using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        // Chọn size/màu nào vào giỏ
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public int Quantity { get; set; }
    }
}