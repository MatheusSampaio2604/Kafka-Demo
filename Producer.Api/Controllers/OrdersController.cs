using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IProducer<Null, string> _stringProducer;
        private readonly IProducer<string, OrderCreated> _avroProducer;

        public OrdersController(
            IProducer<Null, string> stringProducer,
            IProducer<string, OrderCreated> avroProducer)
        {
            _stringProducer = stringProducer;
            _avroProducer = avroProducer;
        }

        [HttpPost("string")]
        public async Task<IActionResult> SendString([FromBody] OrderCreated order)
        {
            var message = JsonSerializer.Serialize(order);
            await _stringProducer.ProduceAsync("orders-string-topic", new() { Value = message });

            return Ok("Enviado como string/JSON");
        }

        [HttpPost("avro")]
        public async Task<IActionResult> SendAvro([FromBody] OrderCreated order)
        {
            var message = new Message<string, OrderCreated>
            {
                Key = order.OrderId.ToString(),
                Value = order
            };

            var result = await _avroProducer.ProduceAsync("orders-avro-topic", message);

            return Ok($"Enviado com Avro! Offset: {result.Offset}");
        }

        [HttpPost("rich")]
        public async Task<IActionResult> SendRich([FromBody] OrderCreated order)
        {
            var headers = new Headers
            {
                {"source", Encoding.UTF8.GetBytes("producer-api-v2") },
                { "tenant", Encoding.UTF8.GetBytes("br") },
                { "event-type", Encoding.UTF8.GetBytes("OrderCreated")  }
            };

            var message = new Message<string, OrderCreated>
            {
                Key = order.CustomerId,
                Value = order,
                Headers = headers
            };

            await _avroProducer.ProduceAsync("orders-enriched-topic", message);

            return Ok("Enviado com headers e chave customizada");
        }
    }
}
