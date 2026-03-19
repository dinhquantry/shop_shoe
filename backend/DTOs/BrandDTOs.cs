namespace backend.DTOs
{
    public class BrandDto
    {
        public int MaTh { get; set; }
        public string TenTh { get; set; } = null!;
        public string? MoTa { get; set; }
        public string? Logo { get; set; } 
    }
    public class BrandCreateDto
    {
        public string TenTh { get; set; } = null!;
        public string? MoTa { get; set; }
       
    }
}