using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class DanhMucRequestDto
    {
        [Required]
        [StringLength(100)]
        public string TenDanhMuc { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class DanhMucDto
    {
        public int Id { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoSanPham { get; set; }
    }
}
