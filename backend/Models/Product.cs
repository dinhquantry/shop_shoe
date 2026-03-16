using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal BasePrice{get;set;}
        public DateTime CreatedAt{get;set;}=DateTime.UtcNow;
        public bool IsDelete{get;set;}=false;
        public int CategoryId{get;set;}
        public int BrandId{get;set;}
        public ICollection<ProductVariant> ProductVariants{get;set;}= new List<ProductVariant>();    

    }
}