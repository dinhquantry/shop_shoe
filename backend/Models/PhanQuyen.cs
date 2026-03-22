using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class PhanQuyen
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TenQuyen { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        public ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
    }
}