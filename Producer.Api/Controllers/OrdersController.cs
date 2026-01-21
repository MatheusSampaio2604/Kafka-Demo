using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Producer.Api.Services;
using Shared.Contracts;
using System.Text;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Producer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderProducer _producer;
        public OrdersController(
            IOrderProducer producer
            )
        {
            _producer = producer;
        }

        [HttpPost("string")]
        public async Task<IActionResult> SendString([FromBody] OrderCreated order)
        {
            await _producer.ProduceAsync(order);

            return Ok("Enviado como string/JSON");
        }

        //[HttpPost("rich")]
        //public async Task<IActionResult> SendRich([FromBody] OrderCreated order)
        //{
        //    var headers = new Headers
        //    {
        //        {"source", Encoding.UTF8.GetBytes("producer-api-v2") },
        //        { "tenant", Encoding.UTF8.GetBytes("br") },
        //        { "event-type", Encoding.UTF8.GetBytes("OrderCreated")  }
        //    };

        //    var message = new Message<string, OrderCreated>
        //    {
        //        Key = order.CustomerId,
        //        Value = order,
        //        Headers = headers
        //    };

        //    await _avroProducer.ProduceAsync("orders-enriched-topic", message);

        //    return Ok("Enviado com headers e chave customizada");
        //}
    }
}
