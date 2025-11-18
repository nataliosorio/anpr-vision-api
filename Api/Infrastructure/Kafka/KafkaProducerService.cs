using System;
using System.Text.Json;
using Business.Interfaces.Producer;
using Confluent.Kafka;
using Entity.Records;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Kafka;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducerService(
        IConfiguration configuration,
        ILogger<KafkaProducerService> logger)
    {
        _logger = logger;

        // Obtiene el broker desde el .env (Kafka__BootstrapServers)
        var bootstrapServers = configuration["Kafka:BootstrapServers"]
            ?? throw new InvalidOperationException("Kafka:BootstrapServers is missing");

        // Obtiene el tópico de sincronización de cámaras (Kafka__Topics__CameraSync)
        _topic = configuration["Kafka:Topics:CameraSync"]
            ?? throw new InvalidOperationException("Kafka:Topics:CameraSync is missing");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,           // Evita duplicados en reintentos
            MessageTimeoutMs = 5000,
            SocketKeepaliveEnable = true,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();

        _logger.LogInformation("KafkaProducerService inicializado. Topic: {Topic}", _topic);
    }

    public async Task SendCameraSyncAsync(CameraSyncEventRecord eventRecord, CancellationToken cancellationToken = default)
    {
        try
        {
            string json = JsonSerializer.Serialize(eventRecord, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(), // Llave única
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, message, cancellationToken);

            _logger.LogInformation(
                "Kafka Producer => CameraSync enviado a {Topic}. Offset={Offset}, Partition={Partition}, Payload={Payload}",
                deliveryResult.Topic, deliveryResult.Offset, deliveryResult.Partition, json);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Kafka Producer => Error enviando CameraSync");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kafka Producer => Error inesperado");
            throw;
        }
    }
}
