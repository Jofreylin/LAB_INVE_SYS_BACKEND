using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

[Table("Warehouse_Stock")]
public partial class WarehouseStock
{
    [Key]
    [Column("stock_id")]
    public int StockId { get; set; }

    [Column("warehouse_id")]
    public int? WarehouseId { get; set; }

    [Column("product_id")]
    public int? ProductId { get; set; }

    [Column("available_quantity")]
    public int AvailableQuantity { get; set; }

    [Column("reserved_quantity")]
    public int ReservedQuantity { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("WarehouseStocks")]
    public virtual InventoryProduct? Product { get; set; }

    [ForeignKey("WarehouseId")]
    [InverseProperty("WarehouseStocks")]
    public virtual Warehouse? Warehouse { get; set; }
}
