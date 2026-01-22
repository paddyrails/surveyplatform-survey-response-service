namespace SurveyPlatform.SurveyResponseService.Domain.Events;

public sealed class ResponseDeletedEvent(Guid responseId, Guid surveyId, string deletedBy) : DomainEvent
{
    public Guid ResponseId { get; } = responseId;
    public Guid SurveyId { get; } = surveyId;
    public string DeletedBy { get; } = deletedBy;
}
