using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace INVE_SYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _service;
        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Supplier>>> GetList()
        {
            var response = await _service.GetList();
            return Ok(response);
        }

        [HttpGet("{supplierId}")]
        public async Task<ActionResult<Supplier>> GetById(int supplierId)
        {
            var response = await _service.GetById(supplierId);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<SupplierResponse>> Create(SupplierDTO model)
        {
            var response = await _service.Create(model);
            return Ok(response);
        }

        [HttpPut("{supplierId}")]
        public async Task<ActionResult<SupplierResponse>> Update(int supplierId, SupplierDTO model)
        {
            var response = await _service.Update(supplierId, model);
            return Ok(response);
        }

        [HttpDelete("{supplierId}")]
        public async Task<ActionResult<SupplierResponse>> Delete(int supplierId)
        {
            var response = await _service.Delete(supplierId);
            return Ok(response);
        }
    }
}
