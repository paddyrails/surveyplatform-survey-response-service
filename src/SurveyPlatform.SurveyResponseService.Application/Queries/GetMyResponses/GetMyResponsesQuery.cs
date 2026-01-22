using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetMyResponses;

public record GetMyResponsesQuery : IRequest<IReadOnlyList<SurveyResponseDto>>;
