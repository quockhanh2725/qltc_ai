using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace qltc_ai.Models;

public partial class qltcContext : DbContext
{
    public qltcContext()
    {
    }

    public qltcContext(DbContextOptions<qltcContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiLog> AiLog { get; set; }
    public virtual DbSet<AiPhanTich> AiPhanTich { get; set; }
    public virtual DbSet<ChiTietDanhMuc> ChiTietDanhMuc { get; set; }
    public virtual DbSet<Danhmuc> Danhmuc { get; set; }
    public virtual DbSet<Donggop> Donggop { get; set; }
    public virtual DbSet<Giaodich> Giaodich { get; set; }
    public virtual DbSet<Muctieu> Muctieu { get; set; }
    public virtual DbSet<Ngansach> Ngansach { get; set; }
    public virtual DbSet<Nguoidung> Nguoidung { get; set; }
    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<Taikhoan> Taikhoan { get; set; }
    public virtual DbSet<Thongbao> Thongbao { get; set; }
    public virtual DbSet<ThongKeChiTieu> ThongKeChiTieu { get; set; }
    public virtual DbSet<Tinnhan> Tinnhan { get; set; }
    public virtual DbSet<Trochuyen> Trochuyen { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        
        modelBuilder.Entity<AiLog>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PRIMARY");
            entity.ToTable("ai_log");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdLog).HasColumnType("int(11)").HasColumnName("idLog");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.CauHoi).HasMaxLength(255).HasColumnName("cauHoi");
            entity.Property(e => e.TraLoi).HasMaxLength(255).HasColumnName("traLoi");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.AiLog)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("ai_log_ibfk_1");
        });

        
        modelBuilder.Entity<AiPhanTich>(entity =>
        {
            entity.HasKey(e => e.IdPhanTich).HasName("PRIMARY");
            entity.ToTable("ai_phantich");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdPhanTich).HasColumnType("int(11)").HasColumnName("idPhanTich");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.LoaiPhanTich).HasMaxLength(255).HasColumnName("loaiPhanTich");
            entity.Property(e => e.KetQua).HasMaxLength(255).HasColumnName("ketQua");
            entity.Property(e => e.DeXuat).HasMaxLength(255).HasColumnName("deXuat");
            entity.Property(e => e.DoTinCay).HasPrecision(5, 2).HasColumnName("doTinCay");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.AiPhanTich)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("ai_phantich_ibfk_1");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.IdDanhMuc).HasName("PRIMARY");
            entity.ToTable("danhmuc");

            entity.Property(e => e.IdDanhMuc).HasColumnType("int(11)").HasColumnName("idDanhMuc");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(255).HasColumnName("tenDanhMuc");
            entity.Property(e => e.Mau).HasMaxLength(7).HasColumnName("mau");
            entity.Property(e => e.LoaiDanhMuc)
                .HasColumnType("enum('ThuNhap','ChiTieu')")
                .HasColumnName("loaiDanhMuc");
        });

        
        modelBuilder.Entity<ChiTietDanhMuc>(entity =>
        {
            entity.HasKey(e => e.IdChiTiet).HasName("PRIMARY");
            entity.ToTable("chitietsanhmuc");
            entity.HasIndex(e => e.IdDanhMuc, "idDanhMuc");
            entity.HasIndex(e => e.IdNganSach, "idNganSach");

            entity.Property(e => e.IdChiTiet).HasColumnType("int(11)").HasColumnName("idChiTiet");
            entity.Property(e => e.IdDanhMuc).HasColumnType("int(11)").HasColumnName("idDanhMuc");
            entity.Property(e => e.IdNganSach).HasColumnType("int(11)").HasColumnName("idNganSach");
            entity.Property(e => e.GioiHanTien).HasPrecision(15, 2).HasColumnName("gioiHanTien");
            entity.Property(e => e.TienDaTieu).HasPrecision(15, 2).HasColumnName("tienDaTieu");
            entity.Property(e => e.DanhGia)
                .HasColumnType("enum('Tot','TrungBinh','Xau')")
                .HasColumnName("danhGia");

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.ChiTietDanhMuc)
                .HasForeignKey(d => d.IdDanhMuc)
                .HasConstraintName("chitietsanhmuc_ibfk_1");

            entity.HasOne(d => d.IdNganSachNavigation).WithMany(p => p.ChiTietDanhMuc)
                .HasForeignKey(d => d.IdNganSach)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chitietsanhmuc_ibfk_2");
        });

        
        modelBuilder.Entity<Donggop>(entity =>
        {
            entity.HasKey(e => e.IdDongGop).HasName("PRIMARY");
            entity.ToTable("donggop");
            entity.HasIndex(e => e.IdMucTieu, "idMucTieu");

            entity.Property(e => e.IdDongGop).HasColumnType("int(11)").HasColumnName("idDongGop");
            entity.Property(e => e.IdMucTieu).HasColumnType("int(11)").HasColumnName("idMucTieu");
            entity.Property(e => e.SoTien).HasPrecision(15, 2).HasColumnName("soTien");
            entity.Property(e => e.NgayGop).HasColumnType("datetime").HasColumnName("ngayGop");

            entity.HasOne(d => d.IdMucTieuNavigation).WithMany(p => p.Donggop)
                .HasForeignKey(d => d.IdMucTieu)
                .HasConstraintName("donggop_ibfk_1");
        });

        
        modelBuilder.Entity<Giaodich>(entity =>
        {
            entity.HasKey(e => e.IdGiaoDich).HasName("PRIMARY");
            entity.ToTable("giaodich");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");
            entity.HasIndex(e => e.IdChiTiet, "idChiTiet");

            entity.Property(e => e.IdGiaoDich).HasColumnType("int(11)").HasColumnName("idGiaoDich");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.IdChiTiet).HasColumnType("int(11)").HasColumnName("idChiTiet");
            entity.Property(e => e.Tien).HasPrecision(15, 2).HasColumnName("tien");
            entity.Property(e => e.NoiDung).HasColumnType("text").HasColumnName("noiDung");
            entity.Property(e => e.LoaiGiaoDich)
                .HasColumnType("enum('ThuNhap','ChiTieu')")
                .HasColumnName("loaiGiaoDich");
            entity.Property(e => e.NgayGiaoDich).HasColumnType("datetime").HasColumnName("ngayGiaoDich");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.Giaodich)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("fk_giaodich_taikhoan");

            entity.HasOne(d => d.IdChiTietNavigation).WithMany(p => p.Giaodich)
                .HasForeignKey(d => d.IdChiTiet)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_giaodich_chitiet");
        });

        
        modelBuilder.Entity<Muctieu>(entity =>
        {
            entity.HasKey(e => e.IdMucTieu).HasName("PRIMARY");
            entity.ToTable("muctieu");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdMucTieu).HasColumnType("int(11)").HasColumnName("idMucTieu");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.TenMucTieu).HasMaxLength(255).HasColumnName("tenMucTieu");
            entity.Property(e => e.TienMucTieu).HasPrecision(15, 2).HasColumnName("tienMucTieu");
            entity.Property(e => e.ThoiGianMucTieu).HasColumnType("datetime").HasColumnName("thoiGianMucTieu");
            entity.Property(e => e.TrangThai)
                .HasColumnType("enum('Dang','HoanThanh','Huy')")
                .HasColumnName("trangThai");
            entity.Property(e => e.NoiDung).HasMaxLength(255).HasColumnName("noiDung");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.Muctieu)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("muctieu_ibfk_1");
        });

        
        modelBuilder.Entity<Ngansach>(entity =>
        {
            entity.HasKey(e => e.IdNganSach).HasName("PRIMARY");
            entity.ToTable("ngansach");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdNganSach).HasColumnType("int(11)").HasColumnName("idNganSach");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.TongTien).HasPrecision(15, 2).HasColumnName("tongTien");
            entity.Property(e => e.Thang).HasColumnType("datetime").HasColumnName("thang");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.Ngansach)
                .HasForeignKey(d => d.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_ngansach_taikhoan");
        });

        
        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.IdNguoiDung).HasName("PRIMARY");
            entity.ToTable("nguoidung");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan").IsUnique();

            entity.Property(e => e.IdNguoiDung).HasColumnType("int(11)").HasColumnName("idNguoiDung");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.TenNguoiDung).HasMaxLength(255).HasColumnName("tenNguoiDung");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithOne(p => p.Nguoidung)
                .HasForeignKey<Nguoidung>(d => d.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("nguoidung_ibfk_1");
        });

        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");
            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnType("int(11)").HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasMaxLength(255).HasColumnName("role_name");
        });

        
        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.IdTaiKhoan).HasName("PRIMARY");
            entity.ToTable("taikhoan");
            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.RoleId).HasColumnType("int(11)").HasColumnName("role_id");
            entity.Property(e => e.Email).HasMaxLength(255).HasColumnName("email");
            entity.Property(e => e.MatKhau).HasMaxLength(255).HasColumnName("matKhau");
            entity.Property(e => e.IsActive).HasColumnType("tinyint(4)").HasColumnName("is_active");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.Role).WithMany(p => p.Taikhoan)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("taikhoan_ibfk_1");
        });

        
        modelBuilder.Entity<Thongbao>(entity =>
        {
            entity.HasKey(e => e.IdThongBao).HasName("PRIMARY");
            entity.ToTable("thongbao");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdThongBao).HasColumnType("int(11)").HasColumnName("idThongBao");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.TieuDe).HasMaxLength(255).HasColumnName("tieuDe");
            entity.Property(e => e.NoiDung).HasMaxLength(255).HasColumnName("noiDung");
            entity.Property(e => e.IsRead).HasColumnType("tinyint(4)").HasColumnName("isRead");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.Thongbao)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("thongbao_ibfk_1");
        });

        
        modelBuilder.Entity<ThongKeChiTieu>(entity =>
        {
            entity.HasKey(e => e.IdThongKe).HasName("PRIMARY");
            entity.ToTable("thongkechitieu");
            entity.HasIndex(e => e.IdNguoiDung, "idNguoiDung");

            entity.Property(e => e.IdThongKe).HasColumnType("int(11)").HasColumnName("idThongKe");
            entity.Property(e => e.IdNguoiDung).HasColumnType("int(11)").HasColumnName("idNguoiDung");
            entity.Property(e => e.Thang).HasColumnType("int(11)").HasColumnName("thang");
            entity.Property(e => e.Nam).HasColumnType("int(11)").HasColumnName("nam");
            entity.Property(e => e.TongThu).HasPrecision(15, 2).HasColumnName("tongThu");
            entity.Property(e => e.TongChi).HasPrecision(15, 2).HasColumnName("tongChi");
            entity.Property(e => e.TongTietKiem).HasPrecision(15, 2).HasColumnName("tongTietKiem");
            entity.Property(e => e.SoGiaoDich).HasColumnType("int(11)").HasColumnName("soGiaoDich");
            entity.Property(e => e.DanhMucChiNhieuNhat).HasMaxLength(255).HasColumnName("danhMucChiNhieuNhat");
            entity.Property(e => e.TrangThaiCanhBao)
                .HasColumnType("enum('AnToan','CanhBao','VuotMuc')")
                .HasColumnName("trangThaiCanhBao");
            entity.Property(e => e.MoTaCanhBao).HasMaxLength(255).HasColumnName("moTaCanhBao");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime").HasColumnName("ngayCapNhat");

            entity.HasOne(d => d.IdNguoiDungNavigation).WithMany()
                .HasForeignKey(d => d.IdNguoiDung)
                .HasConstraintName("thongkechitieu_ibfk_1");
        });

        
        modelBuilder.Entity<Tinnhan>(entity =>
        {
            entity.HasKey(e => e.IdTinNhan).HasName("PRIMARY");
            entity.ToTable("tinnhan");
            entity.HasIndex(e => e.IdTroChuyen, "idTroChuyen");

            entity.Property(e => e.IdTinNhan).HasColumnType("int(11)").HasColumnName("idTinNhan");
            entity.Property(e => e.IdTroChuyen).HasColumnType("int(11)").HasColumnName("idTroChuyen");
            entity.Property(e => e.NguoiGui).HasMaxLength(50).HasColumnName("nguoiGui");
            entity.Property(e => e.NoiDung).HasMaxLength(255).HasColumnName("noiDung");
            entity.Property(e => e.ThoiGianGui).HasColumnType("datetime").HasColumnName("thoiGianGui");

            entity.HasOne(d => d.IdTroChuyenNavigation).WithMany(p => p.Tinnhan)
                .HasForeignKey(d => d.IdTroChuyen)
                .HasConstraintName("tinnhan_ibfk_1");
        });

        
        modelBuilder.Entity<Trochuyen>(entity =>
        {
            entity.HasKey(e => e.IdTroChuyen).HasName("PRIMARY");
            entity.ToTable("trochuyen");
            entity.HasIndex(e => e.IdTaiKhoan, "idTaiKhoan");

            entity.Property(e => e.IdTroChuyen).HasColumnType("int(11)").HasColumnName("idTroChuyen");
            entity.Property(e => e.IdTaiKhoan).HasColumnType("int(11)").HasColumnName("idTaiKhoan");
            entity.Property(e => e.TrangThai)
                .HasColumnType("enum('DangHoatDong','DaDong')")
                .HasColumnName("trangThai");
            entity.Property(e => e.NgayTao).HasColumnType("datetime").HasColumnName("ngayTao");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.Trochuyen)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("trochuyen_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
