using AutoMapper;
using INVE_SYS.Context;
using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static INVE_SYS.Utilities.Enums;

namespace INVE_SYS.Services
{
    public interface IInventoryService
    {
        Task<InventoryMovementResponse> Inbound(InboundDTO model);
        Task<InventoryMovementResponse> Outbound(OutboundDTO model);
        Task<List<InventoryMovement>> GetMovements();
        Task<ProductMovementStatsDTO> GetMonthlyStats(int? year = null);
    }

    public class InventoryService : IInventoryService
    {
        private readonly INSYContext _context;
        private readonly IMapper _mapper;

        public InventoryService(INSYContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<InventoryMovementResponse> Inbound(InboundDTO model)
        {
            var response = new InventoryMovementResponse();
            try
            {
                var product = await _context.InventoryProducts
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId && p.IsDeleted == false);
                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseId == model.WarehouseId && w.IsDeleted == false);
                if (warehouse == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Almacén no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                var whStock = await _context.WarehouseStocks
                    .FirstOrDefaultAsync(s => s.WarehouseId == model.WarehouseId && s.ProductId == model.ProductId && s.IsDeleted == false);
                if (whStock == null)
                {
                    whStock = new WarehouseStock
                    {
                        WarehouseId = model.WarehouseId,
                        ProductId = model.ProductId,
                        AvailableQuantity = 0,
                        ReservedQuantity = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    _context.WarehouseStocks.Add(whStock);
                    await _context.SaveChangesAsync();
                }

                var movement = new InventoryMovement();
                movement.MovementType = MovementsType.Entry;
                movement.ProductId = model.ProductId;
                movement.WarehouseId = model.WarehouseId;
                movement.Quantity = model.Quantity;
                movement.IsDeleted = false;
                movement.CreatedAt = DateTime.UtcNow;
                movement.UpdatedAt = DateTime.UtcNow;
                movement.ProductExpirationDate = model.ProductExpirationDate;
                movement.PurchasePrice = model.PurchasePrice;
                movement.MovementDate = DateOnly.FromDateTime(DateTime.UtcNow);

                _context.InventoryMovements.Add(movement);
                await _context.SaveChangesAsync();

                response.MovementId = movement.MovementId;
                response.ExecutedAt = movement.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al registrar entrada de productos.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<InventoryMovementResponse> Outbound(OutboundDTO model)
        {
            var response = new InventoryMovementResponse();
            try
            {
                var product = await _context.InventoryProducts
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId && p.IsDeleted == false);
                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseId == model.WarehouseId && w.IsDeleted == false);
                if (warehouse == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Almacén no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }

                var movement = new InventoryMovement();
                movement.MovementType = MovementsType.Exit;
                movement.ProductId = model.ProductId;
                movement.WarehouseId = model.WarehouseId;
                movement.Quantity = model.Quantity;
                movement.IsDeleted = false;
                movement.CreatedAt = DateTime.UtcNow;
                movement.UpdatedAt = DateTime.UtcNow;
                movement.MovementDate = DateOnly.FromDateTime(DateTime.UtcNow);

                _context.InventoryMovements.Add(movement);
                await _context.SaveChangesAsync();

                response.MovementId = movement.MovementId;
                response.ExecutedAt = movement.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al registrar salida de productos.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<List<InventoryMovement>> GetMovements()
        {
            var response = new List<InventoryMovement>();
            try
            {
                response = await _context.InventoryMovements
                    .AsNoTracking()
                    .Include(i => i.Product)
                    .ThenInclude(p => p.Supplier)
                    .Where(m => m.IsDeleted == false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al listar movimientos de inventario.",
                    new { },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<ProductMovementStatsDTO> GetMonthlyStats(int? year = null)
        {
            var response = new ProductMovementStatsDTO();
            try
            {
                // Si no se proporciona un año, usar el año actual
                int targetYear = year ?? DateTime.Now.Year;

                // Obtener todos los movimientos del año especificado
                var movements = await _context.InventoryMovements
                    .AsNoTracking()
                    .Where(m => m.IsDeleted == false && 
                           m.MovementDate.Year == targetYear)
                    .ToListAsync();

                // Agrupar por mes y tipo de movimiento
                var monthlyData = movements
                    .GroupBy(m => new { m.MovementDate.Month })
                    .Select(g => new
                    {
                        Month = g.Key.Month,
                        TotalInbound = g.Where(m => m.MovementType == Enums.MovementsType.Entry).Sum(m => m.Quantity),
                        TotalOutbound = g.Where(m => m.MovementType == Enums.MovementsType.Exit).Sum(m => m.Quantity),
                        TotalPurchaseCost = g.Where(m => m.MovementType == Enums.MovementsType.Entry && m.PurchasePrice.HasValue)
                            .Sum(m => m.PurchasePrice.Value * m.Quantity)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                // Crear estadísticas para todos los meses (1-12), incluso si no hay datos
                for (int month = 1; month <= 12; month++)
                {
                    var monthData = monthlyData.FirstOrDefault(m => m.Month == month);
                    
                    response.MonthlyStats.Add(new MonthlyStatsDTO
                    {
                        Year = targetYear,
                        Month = month,
                        MonthName = System.Globalization.CultureInfo.CreateSpecificCulture("es-ES")
                            .DateTimeFormat.GetMonthName(month),
                        TotalInbound = monthData?.TotalInbound ?? 0,
                        TotalOutbound = monthData?.TotalOutbound ?? 0,
                        TotalPurchaseCost = monthData?.TotalPurchaseCost ?? 0
                    });
                }
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al obtener estadísticas mensuales de movimientos.",
                    new { year },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }
    }
}
