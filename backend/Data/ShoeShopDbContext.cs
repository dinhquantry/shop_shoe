using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data;

public partial class ShoeShopDbContext : DbContext
{
    public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BienTheSanPham> BienTheSanPhams { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<HinhAnhSanPham> HinhAnhSanPhams { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<MauSac> MauSacs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BienTheSanPham>(entity =>
        {
            entity.HasKey(e => e.MaBienThe).HasName("PK__BienTheS__3987CEF5A1B8DB4A");

            entity.ToTable("BienTheSanPham");

            entity.HasIndex(e => e.Sku, "UQ__BienTheS__CA1ECF0DE6681467").IsUnique();

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.SoLuongTon).HasDefaultValue(0);
            entity.Property(e => e.TrangThai).HasDefaultValue((byte)1);

            entity.HasOne(d => d.MaMauNavigation).WithMany(p => p.BienTheSanPhams)
                .HasForeignKey(d => d.MaMau)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__MaMau__6CD828CA");

            entity.HasOne(d => d.MaSizeNavigation).WithMany(p => p.BienTheSanPhams)
                .HasForeignKey(d => d.MaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSa__MaSiz__6BE40491");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.BienTheSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BienTheSan__MaSP__6AEFE058");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaBienThe }).HasName("PK__ChiTietH__64BDDA0FB7B7FCAF");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.DonGia).HasColumnType("decimal(15, 2)");

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaBienThe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHo__MaBie__09746778");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK__ChiTietHoa__MaHD__0880433F");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.MaDg).HasName("PK__DanhGia__2725866017673ADB");

            entity.Property(e => e.MaDg).HasColumnName("MaDG");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.NgayDanhGia)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__DanhGia__MaND__02C769E9");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__DanhGia__MaSP__03BB8E22");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMuc__2725866E2A535CF3");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenDm)
                .HasMaxLength(100)
                .HasColumnName("TenDM");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGh).HasName("PK__GioHang__2725AE85AD729072");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaGh).HasColumnName("MaGH");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaBienTheNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaBienThe)
                .HasConstraintName("FK__GioHang__MaBienT__7E02B4CC");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaND__7D0E9093");
        });

        modelBuilder.Entity<HinhAnhSanPham>(entity =>
        {
            entity.HasKey(e => e.MaAnh).HasName("PK__HinhAnhS__356240DF4B60BDD5");

            entity.ToTable("HinhAnhSanPham");

            entity.Property(e => e.AnhChinh).HasDefaultValue((byte)0);
            entity.Property(e => e.DuongDan)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.HinhAnhSanPhams)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__HinhAnhSan__MaSP__70A8B9AE");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6E0668010DC");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.NgayLap, "IX_HoaDon_NgayLap").IsDescending();

            entity.Property(e => e.MaHd).HasColumnName("MaHD");
            entity.Property(e => e.DiaChiNhanHang).HasMaxLength(255);
            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.NgayLap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.SdtkhachHang)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDTKhachHang");
            entity.Property(e => e.TongTien).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("ChoXacNhan");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKm)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HoaDon__MaKM__7849DB76");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HoaDon__MaND__7755B73D");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KhuyenMa__2725CF15433BF355");

            entity.ToTable("KhuyenMai");

            entity.HasIndex(e => e.MaCode, "UQ__KhuyenMa__152C7C5C07217694").IsUnique();

            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GiamToiDa).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.MaCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(100);
            entity.Property(e => e.TrangThai).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<MauSac>(entity =>
        {
            entity.HasKey(e => e.MaMau).HasName("PK__MauSac__3A5BBB7D2584B1EF");

            entity.ToTable("MauSac");

            entity.Property(e => e.MaHex)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenMau).HasMaxLength(50);
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NguoiDun__2725D7240AAC6631");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.Email, "IX_NguoiDung_Email");

            entity.HasIndex(e => e.Sdt, "IX_NguoiDung_SDT");

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D105344D021F42").IsUnique();

            entity.HasIndex(e => e.Sdt, "UQ__NguoiDun__CA1930A5FBEA2EBE").IsUnique();

            entity.HasIndex(e => e.TaiKhoan, "UQ__NguoiDun__D5B8C7F06B30EB4F").IsUnique();

            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ho).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TaiKhoan)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue((byte)1);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaQuyenNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.MaQuyen)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__NguoiDung__MaQuy__5AB9788F");
        });

        modelBuilder.Entity<PhanQuyen>(entity =>
        {
            entity.HasKey(e => e.MaQuyen).HasName("PK__PhanQuye__1D4B7ED429FA36F2");

            entity.ToTable("PhanQuyen");

            entity.Property(e => e.ChiTiet).HasMaxLength(255);
            entity.Property(e => e.TenQuyen).HasMaxLength(50);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081C3EB0B950");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.TenSp, "IX_SanPham_TenSP");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.Active).HasDefaultValue((byte)1);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DonGia).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.GiaKhuyenMai).HasColumnType("decimal(15, 2)");
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.TenSp)
                .HasMaxLength(200)
                .HasColumnName("TenSP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .HasConstraintName("FK__SanPham__MaDM__625A9A57");

            entity.HasOne(d => d.MaThNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaTh)
                .HasConstraintName("FK__SanPham__MaTH__634EBE90");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.MaSize).HasName("PK__Size__A787E7ED9A6DC1A5");

            entity.ToTable("Size");

            entity.Property(e => e.TenSize).HasMaxLength(20);
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaTh).HasName("PK__ThuongHi__27250075F3EE0628");

            entity.ToTable("ThuongHieu");

            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.Logo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenTh)
                .HasMaxLength(100)
                .HasColumnName("TenTH");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
