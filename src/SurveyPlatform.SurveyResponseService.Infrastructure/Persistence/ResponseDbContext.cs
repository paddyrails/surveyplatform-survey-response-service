using Microsoft.EntityFrameworkCore;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Persistence;

public class ResponseDbContext(DbContextOptions<ResponseDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<SurveyResponse> Responses => Set<SurveyResponse>();
    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.HasDefaultSchema("survey_response");
        mb.ApplyConfigurationsFromAssembly(typeof(ResponseDbContext).Assembly);
    }
}
