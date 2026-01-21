using Confluent.SchemaRegistry;
using Producer.Api.Configuration;
using Producer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1. Configuração do Kafka
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));
var kafkaOptions = builder.Configuration.GetSection("Kafka").Get<KafkaOptions>()!;

// 2. Schema Registry
builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
    new CachedSchemaRegistryClient(new SchemaRegistryConfig
    {
        Url = kafkaOptions.SchemaRegistryUrl
    }));

// 3. Producers (separados!)
builder.Services.AddSingleton<IOrderProducer, OrderProducer>();
builder.Services.AddSingleton<IRetryProducer, RetryProducer>();

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
