namespace SurveyPlatform.SurveyResponseService.Domain.Exceptions;

public class UnauthorizedResponseAccessException() 
    : DomainException("You do not have permission to access this response.");
