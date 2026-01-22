using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseById;

public record GetResponseByIdQuery(Guid ResponseId, bool IncludeAnswers = true) : IRequest<SurveyResponseDetailDto?>;
