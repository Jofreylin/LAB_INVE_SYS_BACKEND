using AutoMapper;
using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Utilities;
using Microsoft.EntityFrameworkCore;
using INVE_SYS.Context;
using System.Net;
using static INVE_SYS.Utilities.Enums;

namespace INVE_SYS.Services
{
    public interface IInventoryProductService
    {
        Task<List<InventoryProduct>> GetList();
        Task<InventoryProduct> GetById(int productId);
        Task<InventoryProductResponse> Create(InventoryProductDTO model);
        Task<InventoryProductResponse> Update(int productId, InventoryProductDTO model);
        Task<InventoryProductResponse> Delete(int productId);
        Task<ProductStockResponse> GetStock(int productId);
        Task<List<ProductAvailabilityResponse>> GetAvailability();
    }

    public class InventoryProductService : IInventoryProductService
    {
        private readonly INSYContext _context;
        private readonly IMapper _mapper;

        public InventoryProductService(INSYContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<InventoryProduct>> GetList()
        {
            var response = new List<InventoryProduct>();

            try
            {
                var products = await _context.InventoryProducts
                    .Where(p => p.IsDeleted == false) // Excluir productos eliminados
                    .ToListAsync();

                response = products;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener la lista de productos.",
                    new {},
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }

            return response;
        }

        public async Task<InventoryProduct> GetById(int productId)
        {
            var response = new InventoryProduct();

            try
            {
                var product = await _context.InventoryProducts
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);

                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }

                response = product;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al obtener el producto.",
                    new { productId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }

            return response;
        }

        // Crear un nuevo producto
        public async Task<InventoryProductResponse> Create(InventoryProductDTO model)
        {
            var response = new InventoryProductResponse();

            try
            {
                var product = _mapper.Map<InventoryProduct>(model);
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsDeleted = false;

                _context.InventoryProducts.Add(product);
                await _context.SaveChangesAsync();

                response.ProductId = product.ProductId;
                response.ExecutedAt = product.CreatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al crear el producto.",
                    model,
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }

            return response;
        }

        
        public async Task<InventoryProductResponse> Update(int productId, InventoryProductDTO model)
        {
            var response = new InventoryProductResponse();

            try
            {
                var product = await _context.InventoryProducts
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);

                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }

                
                product.UpdatedAt = DateTime.UtcNow;
                product.Name = model.Name;
                product.Description = model.Description;
                product.CommonPurchasePrice = model.CommonPurchasePrice;
                product.RegularSalePrice = model.RegularSalePrice;
                product.MaxSalePrice = model.MaxSalePrice;
                product.MinSalePrice = model.MinSalePrice;
                product.SupplierId = model.SupplierId;

                await _context.SaveChangesAsync();

                response.ProductId = product.ProductId;
                response.ExecutedAt = product.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al actualizar el producto.",
                    new { productId, model },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }

            return response;
        }

        public async Task<InventoryProductResponse> Delete(int productId)
        {
            var response = new InventoryProductResponse();

            try
            {
                var product = await _context.InventoryProducts
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);

                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }

                product.IsDeleted = true;
                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.ProductId = product.ProductId;
                response.ExecutedAt = product.UpdatedAt;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Ha ocurrido un error inesperado al eliminar el producto.",
                    new { productId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }

            return response;
        }


        public async Task<ProductStockResponse> GetStock(int productId)
        {
            var response = new ProductStockResponse();
            try
            {
                var product = await _context.InventoryProducts
                    .FirstOrDefaultAsync(p => p.ProductId == productId && p.IsDeleted == false);
                if (product == null)
                {
                    throw new CustomException
                    {
                        UserMessage = "Producto no encontrado.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                var stock = await _context.WarehouseStocks
                    .Where(s => s.ProductId == productId && s.IsDeleted == false)
                    .ToListAsync();
                if (!stock.Any())
                {
                    throw new CustomException
                    {
                        UserMessage = "No hay registro de stock para este producto.",
                        StatusCode = HttpStatusCode.NotFound,
                        Type = ExceptionType.Warning
                    };
                }
                var total = stock.Sum(s => s.AvailableQuantity);
                response.ProductId = productId;
                response.ExecutedAt = DateTime.UtcNow;
                response.TotalAvailable = total;
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al consultar stock de producto.",
                    new { productId },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }

        public async Task<List<ProductAvailabilityResponse>> GetAvailability()
        {
            var response = new List<ProductAvailabilityResponse>();
            try
            {
                var products = await _context.InventoryProducts
                    .Where(p => p.IsDeleted == false)
                    .Select(p => new { p.ProductId, p.Name })
                    .ToListAsync();
                var stocks = await _context.WarehouseStocks
                    .Where(s => s.IsDeleted == false)
                    .GroupBy(s => s.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Total = g.Sum(x => x.AvailableQuantity)
                    })
                    .ToListAsync();
                foreach (var p in products)
                {
                    var st = stocks.FirstOrDefault(s => s.ProductId == p.ProductId);
                    response.Add(new ProductAvailabilityResponse
                    {
                        ProductId = p.ProductId,
                        ProductName = p.Name,
                        TotalAvailable = st == null ? 0 : st.Total
                    });
                }
            }
            catch (Exception ex)
            {
                throw Extensions.TransformException(
                    ex,
                    "Error al consultar disponibilidad de productos.",
                    new { },
                    GetType().Name,
                    Extensions.GetCaller()
                );
            }
            return response;
        }
    }
}