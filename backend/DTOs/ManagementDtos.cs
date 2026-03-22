using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class PhanQuyenRequestDto
    {
        [Required]
        [StringLength(50)]
        public string TenQuyen { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }
    }

    public class PhanQuyenDto
    {
        public int Id { get; set; }
        public string TenQuyen { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public int SoNguoiDung { get; set; }
    }

    public class ThuongHieuRequestDto
    {
        [Required]
        [StringLength(100)]
        public string TenThuongHieu { get; set; } = string.Empty;

        [StringLength(255)]
        public string? LogoUrl { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class ThuongHieuDto
    {
        public int Id { get; set; }
        public string TenThuongHieu { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? MoTa { get; set; }
        public bool TrangThai { get; set; }
        public int SoSanPham { get; set; }
    }

    public class KhuyenMaiRequestDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public decimal PhanTramGiam { get; set; }

        public decimal GiamToiDa { get; set; }

        public decimal GiaTriDonToiThieu { get; set; }

        public int SoLuong { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class KhuyenMaiDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal PhanTramGiam { get; set; }
        public decimal GiamToiDa { get; set; }
        public decimal GiaTriDonToiThieu { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public bool TrangThai { get; set; }
        public bool DangHoatDong { get; set; }
    }

    public class BienTheSanPhamRequestDto
    {
        public int MaSanPham { get; set; }

        public int MaSize { get; set; }

        public int MaMau { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        public int SoLuongTon { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class BienTheSanPhamAdminDto
    {
        public int Id { get; set; }
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public int MaSize { get; set; }
        public string TenSize { get; set; } = string.Empty;
        public int MaMau { get; set; }
        public string TenMau { get; set; } = string.Empty;
        public string? MaHex { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public bool TrangThai { get; set; }
    }

    public class HinhAnhSanPhamAdminDto
    {
        public int Id { get; set; }
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int ThuTu { get; set; }
    }

    public class ChiTietHoaDonRequestDto
    {
        public int MaHoaDon { get; set; }

        public int MaBienThe { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
    }

    public class ChiTietHoaDonAdminDto
    {
        public int Id { get; set; }
        public int MaHoaDon { get; set; }
        public int MaBienThe { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string TenSize { get; set; } = string.Empty;
        public string TenMau { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class DanhGiaRequestDto
    {
        public int MaChiTietHoaDon { get; set; }

        public int MaNguoiDung { get; set; }

        public int MaSanPham { get; set; }

        public byte SoSao { get; set; }

        [StringLength(1000)]
        public string? NoiDung { get; set; }

        public DateTime? NgayDanhGia { get; set; }

        public bool TrangThai { get; set; } = true;
    }

    public class DanhGiaDto
    {
        public int Id { get; set; }
        public int MaChiTietHoaDon { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiDung { get; set; } = string.Empty;
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public byte SoSao { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public bool TrangThai { get; set; }
    }
}
