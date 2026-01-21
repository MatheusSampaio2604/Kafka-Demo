using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Consumer.Worker.Configuration;
using Microsoft.Extensions.Options;

namespace Consumer.Worker.Consumers.Common
{
    public abstract class CommonConsumer<T> : BackgroundService where T : class
    {
        public readonly ILogger<CommonConsumer<T>> _logger;
        public readonly IConsumer<string, T> _consumer;

        public readonly KafkaOptions _options;

        public CommonConsumer(
            ILogger<CommonConsumer<T>> logger,
            ISchemaRegistryClient schemaRegistry,
            IOptions<KafkaOptions> config)
        {
            _logger = logger;
            _options = config.Value;

            ConsumerConfig consumerConfig = new()
            {
                BootstrapServers = _options.BootstrapServers,
                GroupId = _options.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };

            _consumer = new ConsumerBuilder<string, T>(consumerConfig)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(new JsonDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                .Build();
        }

        /// <summary>
        /// Begins consuming messages asynchronously until the specified cancellation token is triggered.
        /// </summary>
        /// <remarks>This method does not return until consumption is stopped or cancellation is
        /// requested. Implementations should honor the cancellation token to allow graceful shutdown.</remarks>
        /// <param name="stoppingToken">A cancellation token that can be used to request termination of the consuming operation.</param>
        /// <returns>A task that represents the asynchronous consuming operation. The task completes when consumption stops or
        /// the cancellation token is triggered.</returns>
        public abstract Task StartConsumingAsync(CancellationToken stoppingToken);
    }
}
