using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class PhanQuyen
{
    public int MaQuyen { get; set; }

    public string TenQuyen { get; set; } = null!;

    public string? ChiTiet { get; set; }

    public virtual ICollection<NguoiDung> NguoiDungs { get; set; } = new List<NguoiDung>();
}
