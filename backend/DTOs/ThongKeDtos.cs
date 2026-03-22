namespace backend.DTOs
{
    public class TongDoanhThuDto
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoDonHoanTat { get; set; }
    }

    public class DonHangTheoTrangThaiDto
    {
        public byte TrangThaiDonHang { get; set; }
        public string TenTrangThai { get; set; } = string.Empty;
        public int SoLuongDonHang { get; set; }
    }

    public class TopSanPhamBanChayDto
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? AnhChinh { get; set; }
        public decimal GiaBan { get; set; }
        public bool TrangThai { get; set; }
        public int TongSoLuongDaBan { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoDonHang { get; set; }
        public int TongSoLuongTon { get; set; }
    }

    public class SanPhamSapHetHangDto
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string? AnhChinh { get; set; }
        public bool TrangThai { get; set; }
        public int TongSoLuongTon { get; set; }
        public int NguongCanhBao { get; set; }
        public int SoBienThe { get; set; }
        public int SoBienTheSapHet { get; set; }
    }
}
