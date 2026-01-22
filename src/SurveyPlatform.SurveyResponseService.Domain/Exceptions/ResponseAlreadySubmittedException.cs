namespace SurveyPlatform.SurveyResponseService.Domain.Exceptions;

public class ResponseAlreadySubmittedException(Guid responseId) 
    : DomainException($"Response '{responseId}' has already been submitted and cannot be modified.");
