using AutoMapper;
using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Exceptions;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseById;

public class GetResponseByIdQueryHandler(
    ISurveyResponseRepository repo, 
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetResponseByIdQuery, SurveyResponseDetailDto?>
{
    public async Task<SurveyResponseDetailDto?> Handle(GetResponseByIdQuery req, CancellationToken ct)
    {
        var response = req.IncludeAnswers 
            ? await repo.GetByIdWithAnswersAsync(req.ResponseId, ct)
            : await repo.GetByIdAsync(req.ResponseId, ct);

        if (response == null) return null;

        // Check access: owner, admin, or survey owner can view
        if (!response.IsOwner(currentUser.UserIdGuid) && !currentUser.IsAdmin)
            throw new UnauthorizedResponseAccessException();

        return mapper.Map<SurveyResponseDetailDto>(response);
    }
}
