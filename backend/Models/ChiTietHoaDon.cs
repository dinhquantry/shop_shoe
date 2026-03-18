using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ChiTietHoaDon
{
    public int MaHd { get; set; }

    public int MaBienThe { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;

    public virtual HoaDon MaHdNavigation { get; set; } = null!;
}
