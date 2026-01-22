namespace SurveyPlatform.SurveyResponseService.Domain.Events;

public sealed class ResponseUpdatedEvent(Guid responseId, Guid surveyId) : DomainEvent
{
    public Guid ResponseId { get; } = responseId;
    public Guid SurveyId { get; } = surveyId;
}
