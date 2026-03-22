using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class MauSacRequestDto
    {
        [Required]
        [StringLength(50)]
        public string TenMau { get; set; } = string.Empty;

        [StringLength(7)]
        [RegularExpression("^#([0-9A-Fa-f]{6})$", ErrorMessage = "MaHex phai theo dinh dang #RRGGBB.")]
        public string? MaHex { get; set; }
    }
}
