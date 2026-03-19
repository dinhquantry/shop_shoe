namespace backend.Models;

public partial class GioHang
{
    public int MaGh { get; set; }

    public int MaNd { get; set; }

    public int MaBienThe { get; set; }

    public int SoLuong { get; set; }

    public virtual BienTheSanPham MaBienTheNavigation { get; set; } = null!;

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;
}
