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
    public interface ISupplierService
    {
        Task<List<Supplier>> GetList();
        Task<Supplier> GetById(int supplierId);
        Task<SupplierResponse> Create(SupplierDTO model);
        Task<SupplierResponse> Update(int supplierId, SupplierDTO model);
        Task<SupplierResponse> Delete(int supplierId);
    }

    public class SupplierService : ISupplierService
    {
        private readonly INSYContext _context;
        private readonly IMapper _mapper;

        public SupplierService(INSYContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Supplier>> GetList()
        {
            var response = new List<Supplier>();
            try
            {
                var suppliers = await _context.Suppliers
                    .Where(s => s.IsDeleted == false)
                    .ToListAsync();
                response = suppliers;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener la lista de proveedores.",
                    new { },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<Supplier> GetById(int supplierId)
        {
            var response = new Supplier();
            try
            {
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsDeleted == false);
                if (supplier == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Proveedor no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                response = supplier;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener el proveedor.",
                    new { supplierId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<SupplierResponse> Create(SupplierDTO model)
        {
            var response = new SupplierResponse();
            try
            {
                var supplier = _mapper.Map<Supplier>(model);
                supplier.CreatedAt = DateTime.UtcNow;
                supplier.UpdatedAt = DateTime.UtcNow;
                supplier.IsDeleted = false;

                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                response.SupplierId = supplier.SupplierId;
                response.ExecutedAt = supplier.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al crear el proveedor.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<SupplierResponse> Update(int supplierId, SupplierDTO model)
        {
            var response = new SupplierResponse();
            try
            {
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsDeleted == false);
                if (supplier == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Proveedor no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                supplier.UpdatedAt = DateTime.UtcNow;
                supplier.Name = model.Name;
                supplier.Description = model.Description;

                await _context.SaveChangesAsync();

                response.SupplierId = supplier.SupplierId;
                response.ExecutedAt = supplier.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al actualizar el proveedor.",
                    new { supplierId, model },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<SupplierResponse> Delete(int supplierId)
        {
            var response = new SupplierResponse();
            try
            {
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.SupplierId == supplierId && s.IsDeleted == false);
                if (supplier == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Proveedor no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                supplier.IsDeleted = true;
                supplier.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.SupplierId = supplier.SupplierId;
                response.ExecutedAt = supplier.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al eliminar el proveedor.",
                    new { supplierId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }
    }
}
