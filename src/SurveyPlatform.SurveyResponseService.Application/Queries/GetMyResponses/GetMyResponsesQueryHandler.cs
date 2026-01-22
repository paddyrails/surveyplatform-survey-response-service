using AutoMapper;
using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetMyResponses;

public class GetMyResponsesQueryHandler(
    ISurveyResponseRepository repo, 
    ICurrentUserService currentUser,
    IMapper mapper) : IRequestHandler<GetMyResponsesQuery, IReadOnlyList<SurveyResponseDto>>
{
    public async Task<IReadOnlyList<SurveyResponseDto>> Handle(GetMyResponsesQuery req, CancellationToken ct)
    {
        if (!currentUser.UserIdGuid.HasValue)
            return [];

        var responses = await repo.GetByRespondentIdAsync(currentUser.UserIdGuid.Value, ct);
        return mapper.Map<IReadOnlyList<SurveyResponseDto>>(responses);
    }
}
