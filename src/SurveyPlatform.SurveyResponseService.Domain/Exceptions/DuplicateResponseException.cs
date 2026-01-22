namespace SurveyPlatform.SurveyResponseService.Domain.Exceptions;

public class DuplicateResponseException(Guid surveyId, Guid respondentId) 
    : DomainException($"Respondent '{respondentId}' has already submitted a response for survey '{surveyId}'.");
