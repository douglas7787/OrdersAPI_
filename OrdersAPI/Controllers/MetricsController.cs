using Microsoft.AspNetCore.Mvc;
using OrdersAPI.DTOs;
using OrdersAPI.Services;

namespace OrdersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly IOrderService _service;

        public MetricsController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<MetricsDto>> GetMetrics()
        {
            var metrics = await _service.GetMetricsAsync();
            return Ok(metrics);
        }
    }
}