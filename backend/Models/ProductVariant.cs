using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id{get;set;}
        public int ProductId{get;set;}
        public Product? Product{get;set;}
        [MaxLength(10)]
        public string Size{get;set;}=string.Empty;
        [MaxLength(50)]
        public string Color{get;set;}=string.Empty;
        [MaxLength(50)]
        public string SKU{get;set;}=string.Empty;
        public int Stock{get;set;}
        [Column(TypeName="decimal(18,2)" )]
        public decimal PriceModifier{get;set;}
    }
}