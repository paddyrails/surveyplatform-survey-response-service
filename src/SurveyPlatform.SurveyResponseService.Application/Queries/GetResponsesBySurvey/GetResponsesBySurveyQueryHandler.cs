using AutoMapper;
using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Domain.Enums;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponsesBySurvey;

public class GetResponsesBySurveyQueryHandler(
    ISurveyResponseRepository repo, 
    IMapper mapper) : IRequestHandler<GetResponsesBySurveyQuery, PagedResultDto<SurveyResponseDto>>
{
    public async Task<PagedResultDto<SurveyResponseDto>> Handle(GetResponsesBySurveyQuery req, CancellationToken ct)
    {
        var responses = await repo.GetBySurveyIdAsync(req.SurveyId, req.Page, req.PageSize, ct);
        var totalCount = await repo.GetResponseCountBySurveyAsync(req.SurveyId, ResponseStatus.Submitted, ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)req.PageSize);

        var items = mapper.Map<IReadOnlyList<SurveyResponseDto>>(responses);
        return new PagedResultDto<SurveyResponseDto>(items, totalCount, req.Page, req.PageSize, totalPages);
    }
}
