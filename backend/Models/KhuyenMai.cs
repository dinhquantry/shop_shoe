using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string MaCode { get; set; } = null!;

    public int? PhanTramGiam { get; set; }

    public decimal? GiamToiDa { get; set; }

    public int? SoLuong { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public byte? TrangThai { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
