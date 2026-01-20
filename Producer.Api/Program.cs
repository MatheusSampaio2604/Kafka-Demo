using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9092",
        Acks = Acks.All,
        EnableIdempotence = true, //Exactly once delivery
        MessageSendMaxRetries = 100,
        RetryBackoffMs = 1000,
        MessageTimeoutMs = 30000
    };
    return new ProducerBuilder<Null, string>(config)
        .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e}"))
        .Build();
});

    // Producer com Avro + Schema Registry (melhor prática)
builder.Services.AddSingleton<IProducer<string, OrderCreated>>(sp =>
{
    var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

    var schemaRegistryConfig = new SchemaRegistryConfig
    {
        Url = "http://localhost:8081"
    };

    var avroSerializer = new AvroSerializer<OrderCreated>(
        new CachedSchemaRegistryClient(schemaRegistryConfig));

    return new ProducerBuilder<string, OrderCreated>(config)
        .SetValueSerializer(avroSerializer)
        .Build();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
