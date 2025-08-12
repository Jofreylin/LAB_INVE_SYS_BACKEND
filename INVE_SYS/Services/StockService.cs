using AutoMapper;
using INVE_SYS.Context;
using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Utilities;
using static INVE_SYS.Utilities.Enums;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Services
{
    public interface IStockService
    {
        Task<ReserveStockResponse> Reserve(ReserveStockDTO model);
        Task<ReleaseStockResponse> Release(ReleaseStockDTO model);
        Task<List<ReservationListResponse>> GetReservations();
    }
    public class StockService : IStockService
    {
        private readonly INSYContext _context;
        private readonly IMapper _mapper;

        public StockService(INSYContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReserveStockResponse> Reserve(ReserveStockDTO model)
        {
            var response = new ReserveStockResponse();
            try
            {
                var whStock = await _context.WarehouseStocks
                .FirstOrDefaultAsync(s => s.WarehouseId == model.WarehouseId && s.ProductId == model.ProductId && s.IsDeleted == false);
                if (whStock == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "No hay stock en ese almacén para ese producto.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                if (whStock.AvailableQuantity < model.Quantity)
                {
                    throw new CustomException
                    {
                        UserMessage = "Stock insuficiente para reservar.",
                        StatusCode = HttpStatusCode.Conflict,
                        Type = ExceptionType.Warning
                    };
                }

                var reservation = new Reservation();
                reservation.ProductId = model.ProductId;
                reservation.WarehouseId = model.WarehouseId;
                reservation.ReservedQuantity = model.Quantity;
                reservation.StatusId = CommonConditions.Reserved;
                reservation.ReservationDate = DateTime.UtcNow;
                reservation.CreatedAt = DateTime.UtcNow;
                reservation.UpdatedAt = DateTime.UtcNow;
                reservation.IsDeleted = false;

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                response.ReservationId = reservation.ReservationId;
                response.ExecutedAt = reservation.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al reservar stock.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<ReleaseStockResponse> Release(ReleaseStockDTO model)
        {
            var response = new ReleaseStockResponse();
            try
            {
                var reservation = await _context.Reservations
                    .FirstOrDefaultAsync(r => r.ReservationId == model.ReservationId && r.StatusId != CommonConditions.Cancelled && r.IsDeleted == false);
                if (reservation == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Reservación no encontrada.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                reservation.StatusId = CommonConditions.Cancelled;
                reservation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                response.ReservationId = reservation.ReservationId;
                response.ExecutedAt = reservation.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al liberar stock reservado.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<List<ReservationListResponse>> GetReservations()
        {
            var response = new List<ReservationListResponse>();
            try
            {
                var reservations = await _context.Reservations
                    .Where(r => r.IsDeleted == false)
                    .Include(r => r.Product)
                    .Include(r => r.Warehouse)
                    .ToListAsync();

                foreach (var r in reservations)
                {
                    response.Add(new ReservationListResponse
                    {
                        ReservationId = r.ReservationId,
                        ProductId = r.ProductId ?? 0,
                        ProductName = r.Product?.Name,
                        WarehouseId = r.WarehouseId ?? 0,
                        WarehouseName = r.Warehouse?.Name,
                        ReservedQuantity = r.ReservedQuantity,
                        StatusId = r.StatusId,
                        ReservationDate = r.ReservationDate
                    });
                }
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al consultar reservaciones de stock.",
                    new { },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }
    }
}
