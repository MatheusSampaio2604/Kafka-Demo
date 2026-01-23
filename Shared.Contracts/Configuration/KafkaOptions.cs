namespace Shared.Contracts.Configuration;

public class KafkaOptions
{
    public required string BootstrapServers { get; set; } = "localhost:9092";
    public required string SchemaRegistryUrl { get; set; } = "http://localhost:8081";
    public required string ConsumerGroupId { get; set; } = "Rodeo-Kafka-V1";
    
    
    
    
    
    
    public required ConsumerContexts ConsumerContexts { get; set; }
    public required ProducerContexts ProducerContexts { get; set; }

    public required int TryAgainTimeoutSeconds { get; set; } = 30; // Default timeout in seconds
}

public class ConsumerContexts
{
    //public string CalibrationMap { get; set; }
    public required string MaterialLocation { get; set; }
    //public string MaterialMovement { get; set; }
    //public string VehicleDetection { get; set; }
}

public class ProducerContexts
{
    public required string CalibrationMap { get; set; }
    public required string MaterialLocation { get; set; }
    public required string MaterialMovement { get; set; }
    public required string VehicleDetection { get; set; }
}


