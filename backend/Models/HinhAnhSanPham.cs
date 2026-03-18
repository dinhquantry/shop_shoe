using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class HinhAnhSanPham
{
    public int MaAnh { get; set; }

    public int MaSp { get; set; }

    public string DuongDan { get; set; } = null!;

    public byte? AnhChinh { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
