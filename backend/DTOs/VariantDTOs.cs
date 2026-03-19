namespace backend.DTOs
{
    public class VariantDto
    {
        public int MaBienThe { get; set; }
        public int MaSp { get; set; }
        public string TenSp { get; set; } = null!; // Tên giày
        
        public int MaSize { get; set; }
        public string TenSize { get; set; } = null!; // Chữ "42", "43"
        
        public int MaMau { get; set; }
        public string TenMau { get; set; } = null!;  // Chữ "Đen", "Trắng"
        
        public int? SoLuongTon { get; set; }
        public string? Sku { get; set; } // Mã vạch/Mã kho
        public byte? TrangThai { get; set; }
    }
    public class VariantCreateDto
    {
        public int MaSp { get; set; }
        public int MaSize { get; set; }
        public int MaMau { get; set; }
        public int? SoLuongTon { get; set; }
        public string? Sku { get; set; } 
    }
    public class SizeDto
    {
        public int MaSize { get; set; }
        public string TenSize { get; set; } = null!;
    }
    public class ColorDto
    {
        public int MaMau { get; set; }
        public string TenMau { get; set; } = null!;
        public string? MaHex { get; set; } // Mã màu (Ví dụ: #000000 cho màu đen)
    }
}