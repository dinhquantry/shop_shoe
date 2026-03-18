using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ThuongHieu
{
    public int MaTh { get; set; }

    public string TenTh { get; set; } = null!;

    public string? Logo { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
