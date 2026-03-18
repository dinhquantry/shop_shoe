using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class BienTheSanPham
{
    public int MaBienThe { get; set; }

    public int MaSp { get; set; }

    public int MaSize { get; set; }

    public int MaMau { get; set; }

    public int? SoLuongTon { get; set; }

    public string? Sku { get; set; }

    public byte? TrangThai { get; set; }

    public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual MauSac MaMauNavigation { get; set; } = null!;

    public virtual Size MaSizeNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
