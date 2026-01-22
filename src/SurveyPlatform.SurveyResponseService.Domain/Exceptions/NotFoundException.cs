namespace SurveyPlatform.SurveyResponseService.Domain.Exceptions;

public class NotFoundException(string entity, object key) : DomainException($"Entity '{entity}' with key '{key}' not found.");
