using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

[Table("Inventory_Products")]
public partial class InventoryProduct
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [Unicode(false)]
    public string? Description { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [Column("common_purchase_price", TypeName = "decimal(18, 2)")]
    public decimal? CommonPurchasePrice { get; set; }

    [Column("regular_sale_price", TypeName = "decimal(18, 2)")]
    public decimal? RegularSalePrice { get; set; }

    [Column("max_sale_price", TypeName = "decimal(18, 2)")]
    public decimal? MaxSalePrice { get; set; }

    [Column("min_sale_price", TypeName = "decimal(18, 2)")]
    public decimal? MinSalePrice { get; set; }

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [InverseProperty("Product")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    [InverseProperty("Product")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    [ForeignKey("SupplierId")]
    [InverseProperty("InventoryProducts")]
    public virtual Supplier? Supplier { get; set; }

    [InverseProperty("Product")]
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
}
