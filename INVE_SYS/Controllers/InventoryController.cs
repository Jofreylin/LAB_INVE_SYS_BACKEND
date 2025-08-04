using INVE_SYS.DTO;
using INVE_SYS.Models;
using INVE_SYS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace INVE_SYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        [HttpPost("inbound")]
        public async Task<ActionResult<InventoryMovementResponse>> Inbound(InboundDTO model)
        {
            var response = await _service.Inbound(model);
            return Ok(response);
        }

        [HttpPost("outbound")]
        public async Task<ActionResult<InventoryMovementResponse>> Outbound(OutboundDTO model)
        {
            var response = await _service.Outbound(model);
            return Ok(response);
        }

        [HttpGet("movements")]
        public async Task<ActionResult<List<InventoryMovement>>> GetMovements()
        {
            var response = await _service.GetMovements();
            return Ok(response);
        }
    }
}
