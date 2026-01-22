using MediatR;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.DeleteResponse;

public record DeleteResponseCommand(Guid ResponseId) : IRequest<bool>;
