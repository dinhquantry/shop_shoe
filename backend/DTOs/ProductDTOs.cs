using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal BasePrice { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Giá khuyến mãi phải lớn hơn hoặc bằng 0")]
        public decimal? SalePrice { get; set; }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class ProductUpdateDto : ProductCreateDto
    {
    }
}
