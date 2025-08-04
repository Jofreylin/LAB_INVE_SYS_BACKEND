using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace INVE_SYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _service;
        public WarehouseController(IWarehouseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Warehouse>>> GetList()
        {
            var response = await _service.GetList();
            return Ok(response);
        }

        [HttpGet("{warehouseId}")]
        public async Task<ActionResult<Warehouse>> GetById(int warehouseId)
        {
            var response = await _service.GetById(warehouseId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<WarehouseResponse>> Create(WarehouseDTO model)
        {
            var response = await _service.Create(model);
            return Ok(response);
        }

        [HttpPut("{warehouseId}")]
        public async Task<ActionResult<WarehouseResponse>> Update(int warehouseId, WarehouseDTO model)
        {
            var response = await _service.Update(warehouseId, model);
            return Ok(response);
        }

        [HttpDelete("{warehouseId}")]
        public async Task<ActionResult<WarehouseResponse>> Delete(int warehouseId)
        {
            var response = await _service.Delete(warehouseId);
            return Ok(response);
        }
    }
}
