using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required, MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;
    }
}