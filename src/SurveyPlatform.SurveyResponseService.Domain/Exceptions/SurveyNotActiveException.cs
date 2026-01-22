namespace SurveyPlatform.SurveyResponseService.Domain.Exceptions;

public class SurveyNotActiveException(Guid surveyId) 
    : DomainException($"Survey '{surveyId}' is not active and cannot accept responses.");
