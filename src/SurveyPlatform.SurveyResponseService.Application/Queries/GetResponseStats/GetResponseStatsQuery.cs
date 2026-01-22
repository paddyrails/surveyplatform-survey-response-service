using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseStats;

public record GetResponseStatsQuery(Guid SurveyId) : IRequest<ResponseStatsDto>;
