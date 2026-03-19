using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CategoryDto
    {
        public int MaDm { get; set; }
        public string TenDm { get; set; } = null!;
        public string? MoTa { get; set; }
    }

    public class CategoryCreateDto
    {
        public string TenDm{get;set;}=null!;
        public string? MoTa{get;set;}
    }
}
