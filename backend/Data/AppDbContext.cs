using Microsoft.EntityFrameworkCore;
using backend.Models;
namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PhanQuyen> PhanQuyens { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<ThuongHieu> ThuongHieus { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<MauSac> MauSacs { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<BienTheSanPham> BienTheSanPhams { get; set; }
        public DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }
        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // UNIQUE INDEX
            // =========================

            modelBuilder.Entity<PhanQuyen>()
                .HasIndex(x => x.TenQuyen)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(x => x.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(x => x.SoDienThoai)
                .IsUnique();

            modelBuilder.Entity<DanhMuc>()
                .HasIndex(x => x.TenDanhMuc)
                .IsUnique();

            modelBuilder.Entity<ThuongHieu>()
                .HasIndex(x => x.TenThuongHieu)
                .IsUnique();

            modelBuilder.Entity<Size>()
                .HasIndex(x => x.TenSize)
                .IsUnique();

            modelBuilder.Entity<MauSac>()
                .HasIndex(x => x.TenMau)
                .IsUnique();

            modelBuilder.Entity<KhuyenMai>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<BienTheSanPham>()
                .HasIndex(x => x.SKU)
                .IsUnique();

            modelBuilder.Entity<BienTheSanPham>()
                .HasIndex(x => new { x.MaSanPham, x.MaSize, x.MaMau })
                .IsUnique();

            modelBuilder.Entity<GioHang>()
                .HasIndex(x => new { x.MaNguoiDung, x.MaBienThe })
                .IsUnique();

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasIndex(x => new { x.MaHoaDon, x.MaBienThe })
                .IsUnique();

            modelBuilder.Entity<DanhGia>()
                .HasIndex(x => x.MaChiTietHoaDon)
                .IsUnique();

            // =========================
            // RELATIONSHIPS
            // =========================

            // PhanQuyen - NguoiDung (1 - n)
            modelBuilder.Entity<NguoiDung>()
                .HasOne(x => x.PhanQuyen)
                .WithMany(x => x.NguoiDungs)
                .HasForeignKey(x => x.MaQuyen)
                .OnDelete(DeleteBehavior.Restrict);

            // DanhMuc - SanPham (1 - n)
            modelBuilder.Entity<SanPham>()
                .HasOne(x => x.DanhMuc)
                .WithMany(x => x.SanPhams)
                .HasForeignKey(x => x.MaDanhMuc)
                .OnDelete(DeleteBehavior.Restrict);

            // ThuongHieu - SanPham (1 - n)
            modelBuilder.Entity<SanPham>()
                .HasOne(x => x.ThuongHieu)
                .WithMany(x => x.SanPhams)
                .HasForeignKey(x => x.MaThuongHieu)
                .OnDelete(DeleteBehavior.Restrict);

            // SanPham - BienTheSanPham (1 - n)
            // Đổi sang Restrict để tránh xóa sản phẩm làm ảnh hưởng lịch sử dữ liệu
            modelBuilder.Entity<BienTheSanPham>()
                .HasOne(x => x.SanPham)
                .WithMany(x => x.BienTheSanPhams)
                .HasForeignKey(x => x.MaSanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // Size - BienTheSanPham (1 - n)
            modelBuilder.Entity<BienTheSanPham>()
                .HasOne(x => x.Size)
                .WithMany(x => x.BienTheSanPhams)
                .HasForeignKey(x => x.MaSize)
                .OnDelete(DeleteBehavior.Restrict);

            // MauSac - BienTheSanPham (1 - n)
            modelBuilder.Entity<BienTheSanPham>()
                .HasOne(x => x.MauSac)
                .WithMany(x => x.BienTheSanPhams)
                .HasForeignKey(x => x.MaMau)
                .OnDelete(DeleteBehavior.Restrict);

            // SanPham - HinhAnhSanPham (1 - n)
            // Ảnh là dữ liệu phụ thuộc, có thể xóa theo sản phẩm
            modelBuilder.Entity<HinhAnhSanPham>()
                .HasOne(x => x.SanPham)
                .WithMany(x => x.HinhAnhSanPhams)
                .HasForeignKey(x => x.MaSanPham)
                .OnDelete(DeleteBehavior.Cascade);

            // NguoiDung - GioHang (1 - n)
            // Có thể để Cascade vì giỏ hàng là dữ liệu tạm
            modelBuilder.Entity<GioHang>()
                .HasOne(x => x.NguoiDung)
                .WithMany(x => x.GioHangs)
                .HasForeignKey(x => x.MaNguoiDung)
                .OnDelete(DeleteBehavior.Cascade);

            // BienTheSanPham - GioHang (1 - n)
            modelBuilder.Entity<GioHang>()
                .HasOne(x => x.BienTheSanPham)
                .WithMany(x => x.GioHangs)
                .HasForeignKey(x => x.MaBienThe)
                .OnDelete(DeleteBehavior.Restrict);

            // NguoiDung - HoaDon (1 - n)
            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.NguoiDung)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaNguoiDung)
                .OnDelete(DeleteBehavior.Restrict);

            // KhuyenMai - HoaDon (1 - n)
            // Nếu xóa khuyến mãi thì hóa đơn cũ vẫn tồn tại, FK thành null
            modelBuilder.Entity<HoaDon>()
                .HasOne(x => x.KhuyenMai)
                .WithMany(x => x.HoaDons)
                .HasForeignKey(x => x.MaKhuyenMai)
                .OnDelete(DeleteBehavior.SetNull);

            // HoaDon - ChiTietHoaDon (1 - n)
            // Nếu xóa hóa đơn thì xóa luôn chi tiết hóa đơn
            // Tuy nhiên thực tế thường không xóa hóa đơn, nên quan hệ này vẫn an toàn
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.HoaDon)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.MaHoaDon)
                .OnDelete(DeleteBehavior.Cascade);

            // BienTheSanPham - ChiTietHoaDon (1 - n)
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(x => x.BienTheSanPham)
                .WithMany(x => x.ChiTietHoaDons)
                .HasForeignKey(x => x.MaBienThe)
                .OnDelete(DeleteBehavior.Restrict);

            // ChiTietHoaDon - DanhGia (1 - 1)
            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.ChiTietHoaDon)
                .WithOne(x => x.DanhGia)
                .HasForeignKey<DanhGia>(x => x.MaChiTietHoaDon)
                .OnDelete(DeleteBehavior.Restrict);

            // NguoiDung - DanhGia (1 - n)
            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.NguoiDung)
                .WithMany(x => x.DanhGias)
                .HasForeignKey(x => x.MaNguoiDung)
                .OnDelete(DeleteBehavior.Restrict);

            // SanPham - DanhGia (1 - n)
            modelBuilder.Entity<DanhGia>()
                .HasOne(x => x.SanPham)
                .WithMany(x => x.DanhGias)
                .HasForeignKey(x => x.MaSanPham)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // DECIMAL PRECISION
            // =========================

            modelBuilder.Entity<KhuyenMai>()
                .Property(x => x.PhanTramGiam)
                .HasPrecision(5, 2);

            modelBuilder.Entity<KhuyenMai>()
                .Property(x => x.GiamToiDa)
                .HasPrecision(18, 2);

            modelBuilder.Entity<KhuyenMai>()
                .Property(x => x.GiaTriDonToiThieu)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SanPham>()
                .Property(x => x.GiaBan)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.TamTinh)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.SoTienGiam)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.PhiVanChuyen)
                .HasPrecision(18, 2);

            modelBuilder.Entity<HoaDon>()
                .Property(x => x.TongTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(x => x.DonGia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(x => x.ThanhTien)
                .HasPrecision(18, 2);

            // =========================
            // CHECK CONSTRAINTS
            // =========================

            modelBuilder.Entity<NguoiDung>()
                .ToTable(t => t.HasCheckConstraint("CK_NguoiDung_TrangThai", "[TrangThai] IN (0,1)"));

            modelBuilder.Entity<KhuyenMai>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_KhuyenMai_PhanTramGiam", "[PhanTramGiam] > 0 AND [PhanTramGiam] <= 100");
                    t.HasCheckConstraint("CK_KhuyenMai_GiamToiDa", "[GiamToiDa] >= 0");
                    t.HasCheckConstraint("CK_KhuyenMai_GiaTriDonToiThieu", "[GiaTriDonToiThieu] >= 0");
                    t.HasCheckConstraint("CK_KhuyenMai_SoLuong", "[SoLuong] >= 0");
                    t.HasCheckConstraint("CK_KhuyenMai_ThoiGian", "[NgayBatDau] < [NgayKetThuc]");
                });

            modelBuilder.Entity<SanPham>()
                .ToTable(t => t.HasCheckConstraint("CK_SanPham_GiaBan", "[GiaBan] >= 0"));

            modelBuilder.Entity<BienTheSanPham>()
                .ToTable(t => t.HasCheckConstraint("CK_BienTheSanPham_SoLuongTon", "[SoLuongTon] >= 0"));

            modelBuilder.Entity<HinhAnhSanPham>()
                .ToTable(t => t.HasCheckConstraint("CK_HinhAnhSanPham_ThuTu", "[ThuTu] >= 0"));

            modelBuilder.Entity<GioHang>()
                .ToTable(t => t.HasCheckConstraint("CK_GioHang_SoLuong", "[SoLuong] > 0"));

            modelBuilder.Entity<HoaDon>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_HoaDon_TamTinh", "[TamTinh] >= 0");
                    t.HasCheckConstraint("CK_HoaDon_SoTienGiam", "[SoTienGiam] >= 0");
                    t.HasCheckConstraint("CK_HoaDon_PhiVanChuyen", "[PhiVanChuyen] >= 0");
                    t.HasCheckConstraint("CK_HoaDon_TongTien", "[TongTien] >= 0");
                    t.HasCheckConstraint("CK_HoaDon_TrangThaiDonHang", "[TrangThaiDonHang] IN (0,1,2,3)");
                    t.HasCheckConstraint("CK_HoaDon_TrangThaiThanhToan", "[TrangThaiThanhToan] IN (0,1)");
                    t.HasCheckConstraint("CK_HoaDon_PhuongThucThanhToan", "[PhuongThucThanhToan] IN (0,1)");
                });

            modelBuilder.Entity<ChiTietHoaDon>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_ChiTietHoaDon_SoLuong", "[SoLuong] > 0");
                    t.HasCheckConstraint("CK_ChiTietHoaDon_DonGia", "[DonGia] >= 0");
                    t.HasCheckConstraint("CK_ChiTietHoaDon_ThanhTien", "[ThanhTien] >= 0");
                });

            modelBuilder.Entity<DanhGia>()
                .ToTable(t => t.HasCheckConstraint("CK_DanhGia_SoSao", "[SoSao] BETWEEN 1 AND 5"));
        }
    }
}