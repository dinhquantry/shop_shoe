using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // Mua cụ thể là size nào, màu nào (trỏ vào ProductVariant, không phải Product)
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public int Quantity { get; set; }

        // Tối ưu: Lưu cứng giá tại thời điểm mua, không bị ảnh hưởng nếu sau này Admin đổi giá sản phẩm
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}