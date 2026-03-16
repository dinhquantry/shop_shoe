using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        // Khóa ngoại trỏ về User (Người mua)
        [Required]
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Trạng thái: Pending, Processing, Shipped, Delivered, Cancelled
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; 

        [Required, MaxLength(500)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "COD";

        // 1 Đơn hàng có nhiều Chi tiết đơn hàng
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}