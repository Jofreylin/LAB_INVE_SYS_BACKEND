using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using INVE_SYS.DTO;
using INVE_SYS.Services;
using INVE_SYS.Utilities;
using INVE_SYS.Models;

namespace INVE_SYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IInventoryProductService _service;

        public ProductController(IInventoryProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryProduct>>> GetList()
        {
            var response = await _service.GetList();
            return Ok(response);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<InventoryProduct>> GetById(int productId)
        {
            var response = await _service.GetById(productId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryProductResponse>> Create(InventoryProductDTO model)
        {
            var response = await _service.Create(model);
            return Ok(response);
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult<InventoryProductResponse>> Update(int productId, InventoryProductDTO model)
        {
            var response = await _service.Update(productId, model);
            return Ok(response);
        }

        
        [HttpDelete("{productId}")]
        public async Task<ActionResult<InventoryProductResponse>> Delete(int productId)
        {
            var response = await _service.Delete(productId);
            return Ok(response);
        }

        [HttpGet("{productId}/stock")]
        public async Task<ActionResult<ProductStockResponse>> GetStock(int productId)
        {
            var response = await _service.GetStock(productId);
            return Ok(response);
        }

        [HttpGet("availability")]
        public async Task<ActionResult<List<ProductAvailabilityResponse>>> GetAvailability()
        {
            var response = await _service.GetAvailability();
            return Ok(response);
        }
    }
}