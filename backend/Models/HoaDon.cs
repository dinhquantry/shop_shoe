using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public int? MaNd { get; set; }

    public int? MaKm { get; set; }

    public DateTime? NgayLap { get; set; }

    public decimal TongTien { get; set; }

    public string? TrangThai { get; set; }

    public string DiaChiNhanHang { get; set; } = null!;

    public string SdtkhachHang { get; set; } = null!;

    public string? PhuongThucThanhToan { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual KhuyenMai? MaKmNavigation { get; set; }

    public virtual NguoiDung? MaNdNavigation { get; set; }
}
