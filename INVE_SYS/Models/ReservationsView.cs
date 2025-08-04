using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Models;

[Keyless]
public partial class ReservationsView
{
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

    [Column("status_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? StatusName { get; set; }

    [Column("reservation_date", TypeName = "datetime")]
    public DateTime ReservationDate { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_deleted")]
    public bool? IsDeleted { get; set; }
}
