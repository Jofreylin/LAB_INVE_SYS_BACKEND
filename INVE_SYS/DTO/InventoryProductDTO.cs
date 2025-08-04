using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace INVE_SYS.DTO;

public class InventoryProductDTO
{
    [StringLength(255)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? CommonPurchasePrice { get; set; }

    public decimal? RegularSalePrice { get; set; }

    public decimal? MaxSalePrice { get; set; }

    public decimal? MinSalePrice { get; set; }

    public int? SupplierId { get; set; }
}

public class InventoryProductResponse
{
    public int? ProductId { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class ProductStockDTO
{
    public int Quantity { get; set; }
}

public class ProductStockResponse
{
    public int? ProductId { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public int? TotalAvailable { get; set; }
}

public class ReserveStockDTO
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class ReserveStockResponse
{
    public int? ReservationId { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class ReleaseStockDTO
{
    public int ReservationId { get; set; }
}

public class ReleaseStockResponse
{
    public int? ReservationId { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class InboundDTO
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public DateOnly ProductExpirationDate { get; set; }
    public decimal PurchasePrice { get; set; }
}

public class OutboundDTO
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class InventoryMovementResponse
{
    public int? MovementId { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class ProductAvailabilityResponse
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int TotalAvailable { get; set; }
}