using INVE_SYS.DTO;
using INVE_SYS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace INVE_SYS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _service;

        public StockController(IStockService service)
        {
            _service = service;
        }

        [HttpPost("reserve")]
        public async Task<ActionResult<ReserveStockResponse>> Reserve(ReserveStockDTO model)
        {
            var response = await _service.Reserve(model);
            return Ok(response);
        }

        [HttpPost("release")]
        public async Task<ActionResult<ReleaseStockResponse>> Release(ReleaseStockDTO model)
        {
            var response = await _service.Release(model);
            return Ok(response);
        }

        [HttpGet("reservations")]
        public async Task<ActionResult<List<ReservationListResponse>>> GetReservations()
        {
            var response = await _service.GetReservations();
            return Ok(response);
        }
    }
}
