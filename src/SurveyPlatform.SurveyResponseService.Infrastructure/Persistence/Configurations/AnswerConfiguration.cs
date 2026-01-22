using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;

namespace SurveyPlatform.SurveyResponseService.Infrastructure.Persistence.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> b)
    {
        b.ToTable("answers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();

        b.Property(x => x.ResponseId).HasColumnName("response_id").IsRequired();
        b.Property(x => x.QuestionId).HasColumnName("question_id").IsRequired();

        b.Property(x => x.TextValue).HasColumnName("text_value").HasMaxLength(4000);
        b.Property(x => x.NumericValue).HasColumnName("numeric_value");
        b.Property(x => x.BooleanValue).HasColumnName("boolean_value");
        b.Property(x => x.DateValue).HasColumnName("date_value");
        b.Property(x => x.SelectedOptions).HasColumnName("selected_options").HasColumnType("jsonb");
        b.Property(x => x.Rating).HasColumnName("rating");
        b.Property(x => x.ScaleValue).HasColumnName("scale_value");

        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        b.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(100).IsRequired();
        b.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);

        b.HasIndex(x => x.ResponseId);
        b.HasIndex(x => x.QuestionId);
        b.HasIndex(x => new { x.ResponseId, x.QuestionId }).IsUnique();
    }
}
