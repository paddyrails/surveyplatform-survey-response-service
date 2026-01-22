using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Events;
using System.Text;
using System.Text.Json;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private const string ExchangeName = "surveyplatform.events";

    public RabbitMqEventPublisher(IConfiguration config, ILogger<RabbitMqEventPublisher> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory
        {
            HostName = config["Messaging:RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(config["Messaging:RabbitMQ:Port"] ?? "5672"),
            UserName = config["Messaging:RabbitMQ:Username"] ?? "guest",
            Password = config["Messaging:RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IDomainEvent
    {
        var routingKey = $"response.{@event.EventType.ToLowerInvariant()}";
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        var props = _channel.CreateBasicProperties();
        props.DeliveryMode = 2;
        props.ContentType = "application/json";
        props.MessageId = @event.EventId.ToString();
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        props.Headers = new Dictionary<string, object> { ["EventType"] = @event.EventType };

        _channel.BasicPublish(ExchangeName, routingKey, props, body);
        _logger.LogInformation("Published {EventType} to {RoutingKey}", @event.EventType, routingKey);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
