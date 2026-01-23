using Microsoft.AspNetCore.Mvc;
using Producer.Api.Services;
using Shared.Contracts;
using Shared.Contracts.Producer.MaterialLocation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Producer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialLocationController : ControllerBase
    {
        private readonly IMaterialLocationProducer _producer;

        public MaterialLocationController(IMaterialLocationProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] object data)
        {
            if (data == null) return NoContent();

            HeaderCommon headers = new()
            {
                Message = "MatLocationSnapshotReq",
                Tenant = "PD-BL",
            };

            MessageData message = new()
            {
                Yard = "",
                Location = null,
            };

            //await _producer.ProduceAsync(headers, message);

            return Ok(await _producer.ProduceAsync(headers, message));
            //return Ok("Success");
        }
    }
}
