using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Consumer.Worker.Configuration;
using Consumer.Worker.Consumers.Common;
using Consumer.Worker.Services;
using Microsoft.Extensions.Options;
using Shared.Contracts;

namespace Consumer.Worker.Consumers
{
    public class OrderConsumer : CommonConsumer<OrderCreated>
    {
        public readonly IMessageRequeuer<OrderCreated> _requeuer;

        private readonly IOrderProcessingService _processingService;

        public OrderConsumer(
            ILogger<OrderConsumer> logger,
            ISchemaRegistryClient schemaRegistry,
            IMessageRequeuer<OrderCreated> requeuer,
            IOptions<KafkaOptions> config,

            IOrderProcessingService processingService
            )
            : base(logger, schemaRegistry, config)
        {
            _processingService = processingService;
            _requeuer = requeuer;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await StartConsumingAsync(stoppingToken);

        }

        public override async Task StartConsumingAsync(CancellationToken stoppingToken)
        {
            string[] topics =
            [
                 _options.MainTopic,
                 //_options.RetryTopic5s,
                 //_options.RetryTopic30s,
                 //_options.RetryTopic5m,
                 //_options.RetryTopic1h
            ];

            this._consumer.Subscribe(topics);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                try
                {

                    var success = await _processingService.ProcessAsync(result.Message.Value, result.Topic);

                    if (success)
                    {
                        _consumer.StoreOffset(result);
                        _consumer.Commit();
                    }
                    else
                    {
                        //Fazer um requeue da mensagem
                        await _requeuer.RequeueAsync(result, "Processing failed", stoppingToken);
                        //_consumer.Seek(new TopicPartitionOffset(
                        //    result.TopicPartition,
                        //    result.Offset
                        //));
                        await Task.Delay(TimeSpan.FromSeconds(_options.TryAgainTimeoutSeconds), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro crítico no consumer");

                    _consumer.Seek(new TopicPartitionOffset(
                        result.TopicPartition,
                        result.Offset
                    ));

                    await Task.Delay(TimeSpan.FromSeconds(_options.TryAgainTimeoutSeconds), stoppingToken); // backoff
                }
            }
        }

    }
}
