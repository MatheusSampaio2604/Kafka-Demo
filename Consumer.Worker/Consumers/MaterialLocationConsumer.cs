using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Shared.Contracts.Configuration;
using Consumer.Worker.Consumers.Common;
using Microsoft.Extensions.Options;
using Shared.Contracts.Consumer.MaterialLocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer.Worker.Consumers
{
    public class MaterialLocationConsumer : CommonConsumer<Null, MessageData>
    {
        public MaterialLocationConsumer(
            ILogger<CommonConsumer<Null, MessageData>> logger,
            ISchemaRegistryClient schemaRegistry,
            IOptions<KafkaOptions> config)
            : base(logger, schemaRegistry, config)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartConsumingAsync(stoppingToken);
        }

        public override async Task StartConsumingAsync(CancellationToken stoppingToken)
        {
            string topics = _options.ConsumerContexts.MaterialLocation;

            this._consumer.Subscribe(topics);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, MessageData>? result = new ();
                try
                {
                    result = _consumer.Consume(stoppingToken);

                    // Logic of service
                    bool success = true;

                    if (success)
                    {
                        _consumer.StoreOffset(result);
                        _consumer.Commit();
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico no consumer");
                }
            }
        }

    }
}
