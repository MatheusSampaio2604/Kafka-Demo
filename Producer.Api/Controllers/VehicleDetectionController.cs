using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Producer.Api.Services;
using Shared.Contracts;
using Shared.Contracts.Producer.VehicleDetection;
using System.Threading.Tasks;

namespace Producer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleDetectionController : ControllerBase
    {
        private readonly IVehicleDetectionProducer _producer;

        public VehicleDetectionController(IVehicleDetectionProducer producer)
        {
            _producer = producer;
        }


        [HttpPost]
        public async Task<IActionResult> Index([FromBody] dynamic data)
        {
            HeaderCommon headers = new()
            {
                Message = "VehicleDetection",
                Tenant = "OB-BQ", //definir
            };

            MessageData message = new()
            {
                EventTime = data.EventTime,
                Site = data.Site,
                Equipment = data.Equip,
                Device = data.Device,
                Plate = data.Plate,
                VehicleType = data.Vehicle,
                Orientation = data.Orientation,
            };

            return Ok(await _producer.ProduceAsync(headers, message));

        }
    }
}
