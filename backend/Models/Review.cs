using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public AppUser? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Tối ưu: Ràng buộc điểm đánh giá từ 1 đến 5 sao
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public bool IsApproved { get; set; } = false; // Admin duyệt mới hiện lên web

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}