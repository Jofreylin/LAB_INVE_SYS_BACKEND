using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

public partial class Warehouse
{
    [Key]
    [Column("warehouse_id")]
    public int WarehouseId { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("location")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? Location { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [InverseProperty("Warehouse")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    [InverseProperty("Warehouse")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [InverseProperty("Warehouse")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
}
