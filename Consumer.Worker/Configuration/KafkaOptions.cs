namespace Consumer.Worker.Configuration;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string SchemaRegistryUrl { get; set; } = "http://localhost:8081";
    public string ConsumerGroupId { get; set; } = "order-processing-group-v3";
    public string MainTopic { get; set; } = "orders-avro-topic";
    public string RetryTopic5s { get; set; } = "orders-avro-topic-retry-5s";
    public string RetryTopic30s { get; set; } = "orders-avro-topic-retry-30s";
    public string RetryTopic5m { get; set; } = "orders-avro-topic-retry-5m";
    public string RetryTopic1h { get; set; } = "orders-avro-topic-retry-1h";
    public string DltTopic { get; set; } = "orders-avro-topic-dlt";
    public int TryAgainTimeoutSeconds { get; set; } = 30; // Default timeout in seconds

}

