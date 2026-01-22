namespace SurveyPlatform.SurveyResponseService.Domain.Events;

public sealed class ResponseSubmittedEvent(
    Guid responseId, 
    Guid surveyId, 
    Guid? respondentId,
    bool isAnonymous) : DomainEvent
{
    public Guid ResponseId { get; } = responseId;
    public Guid SurveyId { get; } = surveyId;
    public Guid? RespondentId { get; } = respondentId;
    public bool IsAnonymous { get; } = isAnonymous;
}
