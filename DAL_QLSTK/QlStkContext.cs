using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DTO_QLSTK;
namespace DAL_QLSTK;

public partial class QlStkContext : DbContext
{
    public QlStkContext()
    {
    }

    public QlStkContext(DbContextOptions<QlStkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigToithieu> ConfigToithieus { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoaiTietKiem> LoaiTietKiems { get; set; }

    public virtual DbSet<PhieuGui> PhieuGuis { get; set; }

    public virtual DbSet<PhieuRut> PhieuRuts { get; set; }

    public virtual DbSet<SoTietKiem> SoTietKiems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=.\\SqlExpress; Trusted_Connection=Yes; Initial Catalog=QL_STK; TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigToithieu>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CONFIG_TOITHIEU");

            entity.Property(e => e.Ngaygui).HasColumnName("NGAYGUI");
            entity.Property(e => e.Sotiengui).HasColumnName("SOTIENGUI");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.Cccd).HasName("PK_KH");

            entity.ToTable("KHACH_HANG");

            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.Diachi)
                .HasMaxLength(150)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Hoten)
                .HasMaxLength(30)
                .HasColumnName("HOTEN");
        });

        modelBuilder.Entity<LoaiTietKiem>(entity =>
        {
            entity.HasKey(e => e.Kyhan).HasName("PK_LTK");

            entity.ToTable("LOAI_TIET_KIEM");

            entity.Property(e => e.Kyhan)
                .ValueGeneratedNever()
                .HasColumnName("KYHAN");
            entity.Property(e => e.Laisuat).HasColumnName("LAISUAT");
        });

        modelBuilder.Entity<PhieuGui>(entity =>
        {
            entity.HasKey(e => e.Maphieugui).HasName("PK_PG");

            entity.ToTable("PHIEU_GUI");

            entity.Property(e => e.Maphieugui)
                .ValueGeneratedNever()
                .HasColumnName("MAPHIEUGUI");
            entity.Property(e => e.Maso).HasColumnName("MASO");
            entity.Property(e => e.Ngaygui)
                .HasColumnType("date")
                .HasColumnName("NGAYGUI");
            entity.Property(e => e.Sotien).HasColumnName("SOTIEN");

            entity.HasOne(d => d.MasoNavigation).WithMany(p => p.PhieuGuis)
                .HasForeignKey(d => d.Maso)
                .HasConstraintName("FK_PG_STK");
        });

        modelBuilder.Entity<PhieuRut>(entity =>
        {
            entity.HasKey(e => e.Maphieurut).HasName("PK_PR");

            entity.ToTable("PHIEU_RUT");

            entity.Property(e => e.Maphieurut)
                .ValueGeneratedNever()
                .HasColumnName("MAPHIEURUT");
            entity.Property(e => e.Maso).HasColumnName("MASO");
            entity.Property(e => e.Ngayrut)
                .HasColumnType("date")
                .HasColumnName("NGAYRUT");
            entity.Property(e => e.Sotien).HasColumnName("SOTIEN");

            entity.HasOne(d => d.MasoNavigation).WithMany(p => p.PhieuRuts)
                .HasForeignKey(d => d.Maso)
                .HasConstraintName("FK_PR_STK");
        });

        modelBuilder.Entity<SoTietKiem>(entity =>
        {
            entity.HasKey(e => e.Maso).HasName("PK_STK");

            entity.ToTable("SO_TIET_KIEM");

            entity.Property(e => e.Maso)
                .ValueGeneratedNever()
                .HasColumnName("MASO");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.Laisuat).HasColumnName("LAISUAT");
            entity.Property(e => e.Loaitietkiem).HasColumnName("LOAITIETKIEM");
            entity.Property(e => e.Ngaydongso)
                .HasColumnType("date")
                .HasColumnName("NGAYDONGSO");
            entity.Property(e => e.Ngaymoso)
                .HasColumnType("date")
                .HasColumnName("NGAYMOSO");
            entity.Property(e => e.Sodu).HasColumnName("SODU");
            entity.Property(e => e.Songayduocrut).HasColumnName("SONGAYDUOCRUT");
            entity.Property(e => e.Tienguitoithieu).HasColumnName("TIENGUITOITHIEU");
            entity.Property(e => e.Trangthai).HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.CccdNavigation).WithMany(p => p.SoTietKiems)
                .HasForeignKey(d => d.Cccd)
                .HasConstraintName("FK_STK_KH");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
