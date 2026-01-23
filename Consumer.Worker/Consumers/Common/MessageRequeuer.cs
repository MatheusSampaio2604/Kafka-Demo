using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Shared.Contracts.Configuration;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer.Worker.Consumers.Common
{
    public interface IMessageRequeuer<T> where T : class
    {
        Task RequeueAsync(ConsumeResult<string, T> result, string? reason = null, CancellationToken ct = default);
    }

    public class MessageRequeuer<T> : IMessageRequeuer<T> where T : class
    {
        private readonly IProducer<string, T> _producer;
        private readonly KafkaOptions _options;
        private readonly ILogger<MessageRequeuer<T>> _logger;

        public MessageRequeuer(
            ISchemaRegistryClient schemaRegistry,
            IOptions<KafkaOptions> config,
            ILogger<MessageRequeuer<T>> logger)
        {
            _options = config.Value;
            _logger = logger;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _options.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, T>(producerConfig)
                .SetValueSerializer(new JsonSerializer<T>(schemaRegistry, new JsonSerializerConfig
                {
                    AutoRegisterSchemas = false // já foi registrado no producer principal
                }))
                .Build();
        }

        public async Task RequeueAsync(ConsumeResult<string, T> result, string? reason = null, CancellationToken ct = default)
        {
            var currentTopic = result.Topic;

            //var nextTopic = currentTopic switch
            //{
            //    var t when t == _options.MainTopic || t.Contains("enriched") || t.Contains("string") => _options.RetryTopic5s,
            //    var t when t == _options.RetryTopic5s => _options.RetryTopic30s,
            //    var t when t == _options.RetryTopic30s => _options.RetryTopic5m,
            //    var t when t == _options.RetryTopic5m => _options.RetryTopic1h,
            //    var t when t == _options.RetryTopic1h => _options.DltTopic,
            //    _ => _options.DltTopic
            //};

            // Copia e incrementa o retry-count
            var headers = new Headers();
            foreach (Header header in result.Message.Headers.Cast<Header>())
            {
                headers.Add(header);
            }

            var currentCount = 0;
            var retryHeader = headers.FirstOrDefault(h => h.Key == "retry-count");
            if (retryHeader != null)
                _ = int.TryParse(Encoding.UTF8.GetString(retryHeader.GetValueBytes()), out currentCount);

            headers.Remove("retry-count");
            headers.Add("retry-count", Encoding.UTF8.GetBytes((currentCount + 1).ToString()));
            if (reason != null)
                headers.Add("retry-reason", Encoding.UTF8.GetBytes(reason));

            await _producer.ProduceAsync(/*nextTopic*/currentTopic, new Message<string, T>
            {
                Key = result.Message.Key,
                Value = result.Message.Value,
                Headers = headers
            }, ct);

            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning("Message requeued to {NextTopic} | Attempt {Count} | Reason: {Reason}",
                    /*nextTopic*/currentTopic, currentCount + 1, reason ?? "Processing failed");            
        }
    }
}
