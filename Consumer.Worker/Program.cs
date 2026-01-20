using Consumer.Worker;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    return new CachedSchemaRegistryClient(new SchemaRegistryConfig
    {
        Url = "http://localhost:8081"
    });
});

var host = builder.Build();
host.Run();
