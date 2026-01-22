namespace SurveyPlatform.SurveyResponseService.Domain.Events;

public sealed class ResponseDraftSavedEvent(Guid responseId, Guid surveyId, Guid? respondentId) : DomainEvent
{
    public Guid ResponseId { get; } = responseId;
    public Guid SurveyId { get; } = surveyId;
    public Guid? RespondentId { get; } = respondentId;
}
