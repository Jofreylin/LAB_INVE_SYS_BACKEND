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
    }
}
