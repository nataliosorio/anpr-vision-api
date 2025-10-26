using Business.Interfaces.Detection;
using Confluent.Kafka;
using Entity.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Infrastructure.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConsumerConfig _config;
    private readonly string _topic;
    private IConsumer<string, string>? _consumer;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<ConsumerConfig> options,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _config = options.Value;
        _topic = configuration["Kafka:Topic"]
                 ?? throw new InvalidOperationException("Kafka:Topic is missing");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer = new ConsumerBuilder<string, string>(_config).Build();
        _consumer.Subscribe(_topic);

        _logger.LogInformation("KafkaConsumerService iniciado. Esperando mensajes...");

        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    if (result?.Message?.Value is not null)
                    {
                        _logger.LogInformation("Kafka => Mensaje recibido {Value}", result.Message.Value);

                        var evt = JsonSerializer.Deserialize<PlateDetectedEventRecord>(
                            result.Message.Value, _jsonOptions);

                        if (evt is not null)
                        {
                            //  Fire and forget
                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    using var scope = _scopeFactory.CreateScope();
                                    var business = scope.ServiceProvider.GetRequiredService<IVehicleDetectionManagerBusiness>();
                                    await business.ProcessDetectionAsync(evt, stoppingToken);

                                    // confirmamos offset después de procesar
                                    _consumer.Commit(result);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error procesando evento en background");
                                }
                            }, stoppingToken);
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo deserializar el mensaje: {Value}", result.Message.Value);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consumiendo mensaje de Kafka");
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado en Kafka consumer");
                    Thread.Sleep(2000);
                }
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}
