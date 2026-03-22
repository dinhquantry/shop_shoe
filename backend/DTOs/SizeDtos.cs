using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class SizeRequestDto
    {
        [Required]
        [StringLength(20)]
        public string TenSize { get; set; } = string.Empty;
    }
}
