using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.SubmitResponse;

public record SubmitResponseCommand(
    Guid SurveyId,
    bool IsAnonymous,
    List<AnswerInputDto> Answers,
    string? Metadata = null) : IRequest<SurveyResponseDto>;
