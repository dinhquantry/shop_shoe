using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Size
{
    public int MaSize { get; set; }

    public string TenSize { get; set; } = null!;

    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();
}
