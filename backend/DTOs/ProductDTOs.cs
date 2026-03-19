namespace backend.DTOs
{
    public class ProductDto
    {
        public int MaSp { get; set; }
        public int? MaDm { get; set; }
        public int? MaTh { get; set; }
        public string TenSp { get; set; } = null!;
        public decimal DonGia { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public string? MoTa { get; set; }
        public string? TenDm { get; set; }
        public string? TenTh { get; set; }
    }
    public class ProductCreateDto
    {
        public string TenSp { get; set; } = null!;
        public decimal DonGia { get; set; }
        public decimal? GiaKhuyenMai { get; set; }
        public string? MoTa { get; set; }
        public int MaDm { get; set; }
        public int MaTh { get; set; }
    }
    public class ImageUploadDto
    {
        public IFormFile File { get; set; } = null!;
        public bool IsMain { get; set; }
    }

}
