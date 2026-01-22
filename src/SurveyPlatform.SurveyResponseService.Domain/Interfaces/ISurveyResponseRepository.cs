using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Enums;

namespace SurveyPlatform.SurveyResponseService.Domain.Interfaces;

public interface ISurveyResponseRepository
{
    Task<SurveyResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SurveyResponse?> GetByIdWithAnswersAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId, int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<SurveyResponse>> GetByRespondentIdAsync(Guid respondentId, CancellationToken ct = default);
    Task<SurveyResponse?> GetDraftByRespondentAndSurveyAsync(Guid respondentId, Guid surveyId, CancellationToken ct = default);
    Task<bool> HasRespondentSubmittedAsync(Guid respondentId, Guid surveyId, CancellationToken ct = default);
    Task<int> GetResponseCountBySurveyAsync(Guid surveyId, ResponseStatus? status = null, CancellationToken ct = default);
    Task AddAsync(SurveyResponse response, CancellationToken ct = default);
    void Update(SurveyResponse response);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
