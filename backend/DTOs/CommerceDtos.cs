using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class SanPhamRequestDto
    {
        [Required]
        [StringLength(200)]
        public string TenSanPham { get; set; } = string.Empty;

        public int MaDanhMuc { get; set; }

        public int MaThuongHieu { get; set; }

        public decimal GiaBan { get; set; }

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class HinhAnhSanPhamRequestDto
    {
        public int MaSanPham { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public int ThuTu { get; set; }
    }

    public class GioHangRequestDto
    {
        public int MaNguoiDung { get; set; }

        public int MaBienThe { get; set; }

        public int SoLuong { get; set; }
    }

    public class HoaDonCreateRequestDto
    {
        public int MaNguoiDung { get; set; }

        public int? MaKhuyenMai { get; set; }

        [Required]
        [StringLength(100)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoaiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DiaChiNhan { get; set; } = string.Empty;

        [StringLength(255)]
        public string? GhiChu { get; set; }

        public decimal PhiVanChuyen { get; set; }

        public byte PhuongThucThanhToan { get; set; }

        [MinLength(1)]
        public List<HoaDonItemRequestDto> Items { get; set; } = new();
    }

    public class HoaDonItemRequestDto
    {
        public int MaBienThe { get; set; }

        public int SoLuong { get; set; }
    }

    public class HoaDonUpdateRequestDto
    {
        [Required]
        [StringLength(100)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string SoDienThoaiNhan { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string DiaChiNhan { get; set; } = string.Empty;

        [StringLength(255)]
        public string? GhiChu { get; set; }

        public decimal PhiVanChuyen { get; set; }

        public byte TrangThaiDonHang { get; set; }

        public byte TrangThaiThanhToan { get; set; }

        public byte PhuongThucThanhToan { get; set; }

        public DateTime? NgayThanhToan { get; set; }
    }

    public class SanPhamListItemDto
    {
        public int Id { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = string.Empty;
        public int MaThuongHieu { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? AnhChinh { get; set; }
        public int TongSoLuongTon { get; set; }
    }

    public class SanPhamDetailDto : SanPhamListItemDto
    {
        public List<BienTheSanPhamDto> BienThes { get; set; } = new();
        public List<HinhAnhSanPhamDto> HinhAnhs { get; set; } = new();
    }

    public class BienTheSanPhamDto
    {
        public int Id { get; set; }
        public int MaSize { get; set; }
        public string TenSize { get; set; } = string.Empty;
        public int MaMau { get; set; }
        public string TenMau { get; set; } = string.Empty;
        public string? MaHex { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
    }

    public class HinhAnhSanPhamDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int ThuTu { get; set; }
    }

    public class GioHangItemDto
    {
        public int Id { get; set; }
        public int MaNguoiDung { get; set; }
        public int MaBienThe { get; set; }
        public int SoLuong { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string TenSize { get; set; } = string.Empty;
        public string TenMau { get; set; } = string.Empty;
        public string? MaHex { get; set; }
        public string? AnhChinh { get; set; }
        public int SoLuongTon { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class HoaDonDto
    {
        public int Id { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public int? MaKhuyenMai { get; set; }
        public string? CodeKhuyenMai { get; set; }
        public DateTime NgayDat { get; set; }
        public string TenNguoiNhan { get; set; } = string.Empty;
        public string SoDienThoaiNhan { get; set; } = string.Empty;
        public string DiaChiNhan { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public decimal TamTinh { get; set; }
        public decimal SoTienGiam { get; set; }
        public decimal PhiVanChuyen { get; set; }
        public decimal TongTien { get; set; }
        public byte TrangThaiDonHang { get; set; }
        public byte TrangThaiThanhToan { get; set; }
        public byte PhuongThucThanhToan { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public List<HoaDonChiTietDto> ChiTietHoaDons { get; set; } = new();
    }

    public class HoaDonChiTietDto
    {
        public int Id { get; set; }
        public int MaBienThe { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string TenSize { get; set; } = string.Empty;
        public string TenMau { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
