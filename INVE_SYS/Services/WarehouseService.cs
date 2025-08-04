using AutoMapper;
using INVE_SYS.Context;
using INVE_SYS.Models;
using INVE_SYS.Utilities;
using static INVE_SYS.Utilities.Enums;
using System.Net;
using INVE_SYS.DTO;
using Microsoft.EntityFrameworkCore;

namespace INVE_SYS.Services
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetList();
        Task<Warehouse> GetById(int warehouseId);
        Task<WarehouseResponse> Create(WarehouseDTO model);
        Task<WarehouseResponse> Update(int warehouseId, WarehouseDTO model);
        Task<WarehouseResponse> Delete(int warehouseId);
    }

    public class WarehouseService : IWarehouseService
    {
        private readonly INSYContext _context;
        private readonly IMapper _mapper;

        public WarehouseService(INSYContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Warehouse>> GetList()
        {
            var response = new List<Warehouse>();
            try
            {
                var warehouses = await _context.Warehouses
                    .Where(w => w.IsDeleted == false)
                    .ToListAsync();
                response = warehouses;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener la lista de almacenes.",
                    new { },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<Warehouse> GetById(int warehouseId)
        {
            var response = new Warehouse();
            try
            {
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId && w.IsDeleted == false);
                if (warehouse == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Almacén no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                response = warehouse;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener el almacén.",
                    new { warehouseId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<WarehouseResponse> Create(WarehouseDTO model)
        {
            var response = new WarehouseResponse();
            try
            {
                var warehouse = _mapper.Map<Warehouse>(model);
                warehouse.CreatedAt = DateTime.UtcNow;
                warehouse.UpdatedAt = DateTime.UtcNow;
                warehouse.IsDeleted = false;

                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync();

                response.WarehouseId = warehouse.WarehouseId;
                response.ExecutedAt = warehouse.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al crear el almacén.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<WarehouseResponse> Update(int warehouseId, WarehouseDTO model)
        {
            var response = new WarehouseResponse();
            try
            {
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId && w.IsDeleted == false);
                if (warehouse == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Almacén no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                warehouse.UpdatedAt = DateTime.UtcNow;
                warehouse.Name = model.Name;
                warehouse.Location = model.Location;

                await _context.SaveChangesAsync();

                response.WarehouseId = warehouse.WarehouseId;
                response.ExecutedAt = warehouse.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al actualizar el almacén.",
                    new { warehouseId, model },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<WarehouseResponse> Delete(int warehouseId)
        {
            var response = new WarehouseResponse();
            try
            {
                var warehouse = await _context.Warehouses
                    .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId && w.IsDeleted == false);
                if (warehouse == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Almacén no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                warehouse.IsDeleted = true;
                warehouse.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.WarehouseId = warehouse.WarehouseId;
                response.ExecutedAt = warehouse.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al eliminar el almacén.",
                    new { warehouseId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }
    }
}
