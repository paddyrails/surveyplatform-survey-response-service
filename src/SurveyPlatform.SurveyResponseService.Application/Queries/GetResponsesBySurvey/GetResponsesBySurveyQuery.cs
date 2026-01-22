using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponsesBySurvey;

public record GetResponsesBySurveyQuery(
    Guid SurveyId, 
    int Page = 1, 
    int PageSize = 20) : IRequest<PagedResultDto<SurveyResponseDto>>;
