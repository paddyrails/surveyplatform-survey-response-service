using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Persistence.Configurations;

public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
{
    public void Configure(EntityTypeBuilder<SurveyResponse> b)
    {
        b.ToTable("responses");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        b.Property(x => x.SurveyId).HasColumnName("survey_id").IsRequired();
        b.Property(x => x.RespondentId).HasColumnName("respondent_id");
        b.Property(x => x.RespondentEmail).HasColumnName("respondent_email").HasMaxLength(255);
        b.Property(x => x.IsAnonymous).HasColumnName("is_anonymous");
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();

        b.Property(x => x.SubmittedAt).HasColumnName("submitted_at");
        b.Property(x => x.StartedAt).HasColumnName("started_at");
        b.Property(x => x.CompletedAt).HasColumnName("completed_at");
        b.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");

        b.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
        b.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        b.Property(x => x.Metadata).HasColumnName("metadata").HasColumnType("jsonb");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        b.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(100).IsRequired();
        b.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);

        b.HasMany(x => x.Answers).WithOne().HasForeignKey(a => a.ResponseId).OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => x.SurveyId);
        b.HasIndex(x => x.RespondentId);
        b.HasIndex(x => new { x.SurveyId, x.RespondentId });
        b.HasIndex(x => x.Status);

        b.Ignore(x => x.DomainEvents);
    }
}
