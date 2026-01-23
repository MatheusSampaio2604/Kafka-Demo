using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Shared.Contracts.Configuration;
using Shared.Contracts;
using Shared.Contracts.Producer.MaterialLocation;

namespace Producer.Api.Services
{
    public interface IMaterialLocationProducer
    {
        Task<DeliveryResult<Null, MessageData>> ProduceAsync(HeaderCommon header, MessageData message, CancellationToken ct = default);
    }

    public class MaterialLocationProducer : IMaterialLocationProducer
    {
        private readonly IProducer<Null, MessageData> _producer;
        private readonly KafkaOptions _options;

        public MaterialLocationProducer(ISchemaRegistryClient schemaRegistryClient, IOptions<KafkaOptions> options)
        {
            _options = options.Value;
            ProducerConfig producerConfig = new()
            {
                BootstrapServers = _options.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true,
            };
            _producer = new ProducerBuilder<Null, MessageData>(producerConfig)
                .SetValueSerializer(new JsonSerializer<MessageData>(schemaRegistryClient, new JsonSerializerConfig
                {
                    AutoRegisterSchemas = true
                }))
                .Build();
        }

        public async Task<DeliveryResult<Null, MessageData>> ProduceAsync(HeaderCommon header, MessageData message, CancellationToken ct = default)
        {
            var kafkaMessage = new Message<Null, MessageData>
            {
                Headers =
                [
                    new Header(nameof(header.Message), System.Text.Encoding.UTF8.GetBytes(header.Message)),
                    new Header(nameof(header.Sender), System.Text.Encoding.UTF8.GetBytes(header.Sender)),
                    new Header(nameof(header.Timestamp), System.Text.Encoding.UTF8.GetBytes(header.Timestamp)),
                    new Header(nameof(header.Tenant), System.Text.Encoding.UTF8.GetBytes(header.Tenant)),
                ],

                //Key = null,
                Value = message,
                Timestamp = new Timestamp(DateTime.Now)
            };
            return await _producer.ProduceAsync(_options.ProducerContexts.MaterialLocation, kafkaMessage, ct);
        }
    }
}
