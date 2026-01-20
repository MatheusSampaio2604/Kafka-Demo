using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Shared.Contracts;
using System.Text;

namespace Consumer.Worker
{
    public class Worker(ILogger<Worker> logger, ISchemaRegistryClient schemaRegistryClient) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly ISchemaRegistryClient _schemaRegistry = schemaRegistryClient;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var topics = new[]
            {
                "orders-avro-topic",
                "orders-avro-topic-retry-5s",
                "orders-avro-topic-retry-30s",
                "orders-avro-topic-retry-5m",
                "orders-avro-topic-retry-1h"
            };

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "order-processing-group-v3",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false, // controle manual
                EnablePartitionEof = false,
                AllowAutoCreateTopics = false
            };

            using var consumer = new ConsumerBuilder<string, OrderCreated>(config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(new AvroDeserializer<OrderCreated>(_schemaRegistry).AsSyncOverAsync())
                .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Error}", e))
                .SetPartitionsRevokedHandler((c, partitions) => _logger.LogWarning("Partições revogadas: {Partitions}", partitions))
                .Build();

            consumer.Subscribe(topics);

            var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };
            using var producer = new ProducerBuilder<string, OrderCreated>(producerConfig)
                .SetValueSerializer(new AvroSerializer<OrderCreated>(_schemaRegistry))
                .Build();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    var topic = consumeResult.Topic;
                    var order = consumeResult.Message.Value;

                    var success = await ProcessOrder(order, topic);

                    if (success)
                    {
                        consumer.Commit(consumeResult);
                        _logger.LogInformation("Pedido {OrderId} processado com sucesso!", order.OrderId);
                    }
                    else
                    {
                        await ScheduleRetry(consumeResult, producer, topic);
                        consumer.Commit(consumeResult); // evita loop infinito
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado no consumer");
                }
            }
        }

        private async Task<bool> ProcessOrder(OrderCreated order, string currentTopic)
        {
            try
            {
                // Simula falha aleatória nos primeiros retries
                if (currentTopic.Contains("retry") && Random.Shared.NextDouble() < 0.4)
                {
                    _logger.LogWarning("Falha temporária no processamento do pedido {OrderId}", order.OrderId);
                    return false;
                }

                // Simula chamada externa (banco, API, etc)
                await Task.Delay(200);
                _logger.LogInformation("Processamento real do pedido {OrderId} - Total: {Total}", order.OrderId, order.TotalAmount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pedido {OrderId}", order.OrderId);
                return false;
            }
        }

        private async Task ScheduleRetry(ConsumeResult<string, OrderCreated> result, IProducer<string, OrderCreated> producer, string currentTopic)
        {
            var nextTopic = currentTopic switch
            {
                "orders-avro-topic" => "orders-avro-topic-retry-5s",
                "orders-avro-topic-retry-5s" => "orders-avro-topic-retry-30s",
                "orders-avro-topic-retry-30s" => "orders-avro-topic-retry-5m",
                "orders-avro-topic-retry-5m" => "orders-avro-topic-retry-1h",
                "orders-avro-topic-retry-1h" => "orders-avro-topic-dlt",
                _ => "orders-avro-topic-dlt"
            };

            var headers = result.Message.Headers;
            headers.Add("retry-count", Encoding.UTF8.GetBytes(
                (int.Parse(headers.FirstOrDefault(h => h.Key == "retry-count")?.GetValueBytes()?.Length > 0
                    ? Encoding.UTF8.GetString(headers.FirstOrDefault(h => h.Key == "retry-count").GetValueBytes())
                    : "0") + 1).ToString()));

            await producer.ProduceAsync(nextTopic, new Message<string, OrderCreated>
            {
                Key = result.Message.Key,
                Value = result.Message.Value,
                Headers = headers
            });

            _logger.LogWarning("Mensagem reenviada para {NextTopic} - Pedido {OrderId}", nextTopic, result.Message.Value.OrderId);
        }
    }
}