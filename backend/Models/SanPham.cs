using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public int? MaDm { get; set; }

    public int? MaTh { get; set; }

    public string TenSp { get; set; } = null!;

    public decimal DonGia { get; set; }

    public decimal? GiaKhuyenMai { get; set; }

    public string? MoTa { get; set; }

    public byte? Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BienTheSanPham> BienTheSanPhams { get; set; } = new List<BienTheSanPham>();

    public virtual ICollection<DanhGia> DanhGia { get; set; } = new List<DanhGia>();

    public virtual ICollection<HinhAnhSanPham> HinhAnhSanPhams { get; set; } = new List<HinhAnhSanPham>();

    public virtual DanhMuc? MaDmNavigation { get; set; }

    public virtual ThuongHieu? MaThNavigation { get; set; }
}
