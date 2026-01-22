using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Domain.Enums;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseStats;

public class GetResponseStatsQueryHandler(ISurveyResponseRepository repo) 
    : IRequestHandler<GetResponseStatsQuery, ResponseStatsDto>
{
    public async Task<ResponseStatsDto> Handle(GetResponseStatsQuery req, CancellationToken ct)
    {
        var submitted = await repo.GetResponseCountBySurveyAsync(req.SurveyId, ResponseStatus.Submitted, ct);
        var drafts = await repo.GetResponseCountBySurveyAsync(req.SurveyId, ResponseStatus.Draft, ct);

        return new ResponseStatsDto
        {
            SurveyId = req.SurveyId,
            TotalResponses = submitted + drafts,
            SubmittedResponses = submitted,
            DraftResponses = drafts
        };
    }
}
