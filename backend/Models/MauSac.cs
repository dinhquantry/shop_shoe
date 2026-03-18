using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class MauSac
{
    public int MaMau { get; set; }

    public string TenMau { get; set; } = null!;

    public string? MaHex { get; set; }

    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
}
