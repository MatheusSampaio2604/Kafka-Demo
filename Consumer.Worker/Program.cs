using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Consumer.Worker;
using Consumer.Worker.Configuration;
using Consumer.Worker.Consumers;
using Consumer.Worker.Consumers.Common;
using Consumer.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<OrderConsumer>();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
    new CachedSchemaRegistryClient(new SchemaRegistryConfig
    {
        Url = builder.Configuration["Kafka:SchemaRegistryUrl"]
    }));

//builder.Services.AddScoped(typeof(ICommonConsumer<>), typeof(CommonConsumer<>));
builder.Services.AddSingleton(typeof(IMessageRequeuer<>), typeof(MessageRequeuer<>));

builder.Services.AddSingleton<IOrderProcessingService, OrderProcessingService>();

var host = builder.Build();
host.Run();
