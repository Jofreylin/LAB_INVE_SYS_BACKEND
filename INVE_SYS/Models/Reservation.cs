using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

public partial class Reservation
{
    [Key]
    [Column("reservation_id")]
    public int ReservationId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("warehouse_id")]
    public int? WarehouseId { get; set; }

    [Column("reserved_quantity")]
    public int ReservedQuantity { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("reservation_date", TypeName = "datetime")]
    public DateTime ReservationDate { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [InverseProperty("Reservation")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    [ForeignKey("ProductId")]
    [InverseProperty("Reservations")]
    public virtual InventoryProduct? Product { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("Reservations")]
    public virtual Warehouse? Warehouse { get; set; }
}
