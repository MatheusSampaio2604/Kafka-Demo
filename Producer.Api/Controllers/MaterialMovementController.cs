using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Producer.Api.Services;
using Shared.Contracts;
using Shared.Contracts.Producer.MaterialMovement;
using System.Threading.Tasks;

namespace Producer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialMovementController : ControllerBase
    {
        private readonly IMaterialMovementProducer _producer;

        public MaterialMovementController(IMaterialMovementProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] dynamic data)
        {
            HeaderCommon headers = new()
            {
                Message = "MaterialMovement",
                Tenant = "PD-BL"
            };

            MessageData message = new()
            {
                MovementTime = data.MovementTime,
                Yard = data.Yard,
                Location = data.Location,
                X = data.X,
                Y = data.Y,
                Equipment = data.Equipment,
                UserName = data.UserName,
                Materials = data.Materials,
                Warnings = data.Warnings
            };

            return Ok(await _producer.ProduceAsync(headers, message));
        }
    }
}
