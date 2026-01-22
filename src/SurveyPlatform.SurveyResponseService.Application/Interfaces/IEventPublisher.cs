using SurveyPlatform.SurveyResponseService.Domain.Events;

namespace SurveyPlatform.SurveyResponseService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IDomainEvent;
}
