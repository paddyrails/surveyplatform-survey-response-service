using Microsoft.EntityFrameworkCore;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Enums;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;
using SurveyPlatform.SurveyResponseService.Infrastructure.Persistence;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Repositories;

public class SurveyResponseRepository(ResponseDbContext ctx) : ISurveyResponseRepository
{
    public async Task<SurveyResponse?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await ctx.Responses.FirstOrDefaultAsync(r => r.Id == id && r.Status != ResponseStatus.Deleted, ct);

    public async Task<SurveyResponse?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default) =>
        await ctx.Responses
            .Include(r => r.Answers)
            .FirstOrDefaultAsync(r => r.Id == id && r.Status != ResponseStatus.Deleted, ct);

    public async Task<IReadOnlyList<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId, int page, int pageSize, CancellationToken ct = default) =>
        await ctx.Responses
            .Include(r => r.Answers)
            .Where(r => r.SurveyId == surveyId && r.Status == ResponseStatus.Submitted)
            .OrderByDescending(r => r.SubmittedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<SurveyResponse>> GetByRespondentIdAsync(Guid respondentId, CancellationToken ct = default) =>
        await ctx.Responses
            .Include(r => r.Answers)
            .Where(r => r.RespondentId == respondentId && r.Status != ResponseStatus.Deleted)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<SurveyResponse?> GetDraftByRespondentAndSurveyAsync(Guid respondentId, Guid surveyId, CancellationToken ct = default) =>
        await ctx.Responses
            .Include(r => r.Answers)
            .FirstOrDefaultAsync(r => 
                r.RespondentId == respondentId && 
                r.SurveyId == surveyId && 
                r.Status == ResponseStatus.Draft, ct);

    public async Task<bool> HasRespondentSubmittedAsync(Guid respondentId, Guid surveyId, CancellationToken ct = default) =>
        await ctx.Responses.AnyAsync(r => 
            r.RespondentId == respondentId && 
            r.SurveyId == surveyId && 
            r.Status == ResponseStatus.Submitted, ct);

    public async Task<int> GetResponseCountBySurveyAsync(Guid surveyId, ResponseStatus? status = null, CancellationToken ct = default)
    {
        var query = ctx.Responses.Where(r => r.SurveyId == surveyId && r.Status != ResponseStatus.Deleted);
        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);
        return await query.CountAsync(ct);
    }

    public async Task AddAsync(SurveyResponse response, CancellationToken ct = default) =>
        await ctx.Responses.AddAsync(response, ct);

    public void Update(SurveyResponse response) => ctx.Responses.Update(response);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        await ctx.Responses.AnyAsync(r => r.Id == id && r.Status != ResponseStatus.Deleted, ct);
}
