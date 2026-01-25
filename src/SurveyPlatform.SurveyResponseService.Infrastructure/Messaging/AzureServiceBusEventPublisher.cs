using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Events;
using System.Text;
using System.Text.Json;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Messaging;

public class AzureServiceBusEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ILogger<AzureServiceBusEventPublisher> _logger;
    private const string TopicName = "response-events";

    public AzureServiceBusEventPublisher(IConfiguration config, ILogger<AzureServiceBusEventPublisher> logger)
    {
        _logger = logger;
        
        var connectionString = config["Messaging:AzureServiceBus:ConnectionString"];
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Service Bus connection string is not configured. Set 'Messaging:AzureServiceBus:ConnectionString'");
        }

        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(TopicName);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IDomainEvent
    {
        var message = JsonSerializer.Serialize(@event);
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message))
        {
            MessageId = @event.EventId.ToString(),
            Subject = @event.EventType,
            ContentType = "application/json",
            ApplicationProperties =
            {
                ["EventType"] = @event.EventType,
                ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            }
        };

        await _sender.SendMessageAsync(serviceBusMessage, ct);
        _logger.LogInformation("Published {EventType} to topic {TopicName}", @event.EventType, TopicName);
    }

    public async ValueTask DisposeAsync()
    {
        await _sender.DisposeAsync();
        await _client.DisposeAsync();
    }
}
