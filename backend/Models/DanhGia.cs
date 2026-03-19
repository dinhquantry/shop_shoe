using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class DanhGia
{
    public int MaDg { get; set; }

    public int? MaNd { get; set; }

    public int MaSp { get; set; }

    public int? SoSao { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayDanhGia { get; set; }

    public virtual NguoiDung? MaNdNavigation { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
