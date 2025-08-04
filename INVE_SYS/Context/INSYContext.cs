using System;
using System.Collections.Generic;
using INVE_SYS.Models;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Context;

public partial class INSYContext : DbContext
{
    public INSYContext()
    {
    }

    public INSYContext(DbContextOptions<INSYContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<InventoryProduct> InventoryProducts { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationStatus> ReservationStatuses { get; set; }

    public virtual DbSet<ReservationsView> ReservationsViews { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseStock> WarehouseStocks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DBConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(e => e.MovementId).HasName("PK__Inventor__AB1D1022500ECF27");

            entity.ToTable("Inventory_Movements", tb => tb.HasTrigger("TR_Inventory_Movement_Insert"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__produ__5070F446");

            entity.HasOne(d => d.Reservation).WithMany(p => p.InventoryMovements).HasConstraintName("FK__Inventory__reser__52593CB8");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.InventoryMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__wareh__5165187F");
        });

        modelBuilder.Entity<InventoryProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Inventor__47027DF552994DE7");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Supplier).WithMany(p => p.InventoryProducts).HasConstraintName("FK_Inventory_Products_Suppliers");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__31384C2993BE6E2B");

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("TR_Reservation_Delete");
                    tb.HasTrigger("TR_Reservation_Insert");
                    tb.HasTrigger("TR_Reservation_Update");
                });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.Reservations).HasConstraintName("FK__Reservati__produ__49C3F6B7");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Reservations).HasConstraintName("FK__Reservati__wareh__4AB81AF0");
        });

        modelBuilder.Entity<ReservationStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Reservat__3683B531E8F7E96F");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<ReservationsView>(entity =>
        {
            entity.ToView("ReservationsView");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__734FE6BF103B0CEB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<WarehouseStock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("PK__Warehous__E86668627430A754");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.WarehouseStocks).HasConstraintName("FK__Warehouse__produ__4222D4EF");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseStocks).HasConstraintName("FK__Warehouse__wareh__412EB0B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
