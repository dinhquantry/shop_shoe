using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CategoryDto
    {
        // trả data về cho frontend
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
    }
    public class CategoryCreateDto
    {
        //tạo danh mục mới
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đường dẫn (Slug) không được để trống")]
        public string Slug { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }
    public class CategoryUpdateDto : CategoryCreateDto
    {
        //sửa danh mục
        public bool IsActive { get; set; }
    }
}