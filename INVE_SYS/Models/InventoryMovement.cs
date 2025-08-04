using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

[Table("Inventory_Movements")]
public partial class InventoryMovement
{
    [Key]
    [Column("movement_id")]
    public int MovementId { get; set; }

    [Column("movement_type")]
    [StringLength(50)]
    [Unicode(false)]
    public string MovementType { get; set; } = null!;

    [Column("movement_date")]
    public DateOnly MovementDate { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("reservation_id")]
    public int? ReservationId { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("product_expiration_date")]
    public DateOnly? ProductExpirationDate { get; set; }

    [Column("purchase_price", TypeName = "decimal(18, 2)")]
    public decimal? PurchasePrice { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("InventoryMovements")]
    public virtual InventoryProduct Product { get; set; } = null!;

    [ForeignKey("ReservationId")]
    [InverseProperty("InventoryMovements")]
    public virtual Reservation? Reservation { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("InventoryMovements")]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
