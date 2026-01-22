using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.SaveDraft;

public record SaveDraftCommand(
    Guid SurveyId,
    List<AnswerInputDto> Answers,
    string? Metadata = null) : IRequest<SurveyResponseDto>;
