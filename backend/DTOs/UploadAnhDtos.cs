using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.DTOs
{
    public class UploadAnhSanPhamRequestDto
    {
        [Required]
        public int MaSanPham { get; set; }

        [Required]
        [MinLength(1)]
        public List<IFormFile> Files { get; set; } = new();

        public int ThuTuBatDau { get; set; } = 0;

        public int? AnhChinhIndex { get; set; }
    }
}
