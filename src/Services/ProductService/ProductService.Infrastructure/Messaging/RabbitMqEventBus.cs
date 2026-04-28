using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Abstractions;
using RabbitMQ.Client;

namespace ProductService.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus
{
    private readonly IConfiguration _configuration;

    public RabbitMqEventBus(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task PublishAsync<TEvent>(TEvent eventMessage, CancellationToken cancellationToken)
        where TEvent : class
    {
        var hostName = _configuration["RabbitMq:HostName"] ?? "localhost";
        var userName = _configuration["RabbitMq:UserName"] ?? "guest";
        var password = _configuration["RabbitMq:Password"] ?? "guest";
        var exchangeName = _configuration["RabbitMq:ExchangeName"] ?? "kayra_export_events";

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            DispatchConsumersAsync = true
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        var eventName = typeof(TEvent).Name;
        var routingKey = eventName.Replace("IntegrationEvent", string.Empty).ToLowerInvariant();
        var payload = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = eventName;
        properties.MessageId = Guid.NewGuid().ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}