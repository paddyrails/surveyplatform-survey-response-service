using Microsoft.AspNetCore.Http;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using System.Security.Claims;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
{
    public string? UserId => http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    public Guid? UserIdGuid 
    {
        get 
        {
            var userId = UserId;
            return Guid.TryParse(userId, out var guid) ? guid : null;
        }
    }
    
    public string? Email => http.HttpContext?.User?.FindFirst("email")?.Value 
        ?? http.HttpContext?.User?.FindFirst("preferred_username")?.Value;
    
    public bool IsAuthenticated => http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    
    public bool IsAdmin => http.HttpContext?.User?.IsInRole("survey_admin") == true 
        || http.HttpContext?.User?.IsInRole("system_admin") == true;
}
