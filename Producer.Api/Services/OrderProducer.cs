using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Producer.Api.Configuration;
using Shared.Contracts;

namespace Producer.Api.Services;

public interface IOrderProducer
{
    Task<DeliveryResult<string, OrderCreated>> ProduceAsync(OrderCreated order, CancellationToken ct = default);
}
public class OrderProducer : IOrderProducer
{
    private readonly IProducer<string, OrderCreated> _producer;
    private readonly KafkaOptions _options;

    public OrderProducer(ISchemaRegistryClient schemaRegistryClient, IOptions<KafkaOptions> options)
    {
        _options = options.Value;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, OrderCreated>(producerConfig)
            .SetValueSerializer(new JsonSerializer<OrderCreated>(schemaRegistryClient, new JsonSerializerConfig
            {
                AutoRegisterSchemas = true
            }))
            .Build();
    }

    public async Task<DeliveryResult<string, OrderCreated>> ProduceAsync(OrderCreated order, CancellationToken ct = default)
    {
        var message = new Message<string, OrderCreated>
        {
            Key = order.OrderId.ToString(),
            Value = order
        };

        return await _producer.ProduceAsync(_options.MainTopic, message, ct);
    }
}

