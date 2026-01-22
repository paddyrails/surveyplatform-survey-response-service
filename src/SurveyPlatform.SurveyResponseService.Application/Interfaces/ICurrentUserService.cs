namespace SurveyPlatform.SurveyResponseService.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid? UserIdGuid { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}
