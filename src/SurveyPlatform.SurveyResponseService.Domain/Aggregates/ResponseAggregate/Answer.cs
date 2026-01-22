using SurveyPlatform.SurveyResponseService.Domain.Exceptions;

namespace SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;

public class Answer : Entity
{
    public Guid ResponseId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string? TextValue { get; private set; }
    public int? NumericValue { get; private set; }
    public bool? BooleanValue { get; private set; }
    public DateTime? DateValue { get; private set; }
    public string? SelectedOptions { get; private set; } // JSON array of selected option IDs
    public int? Rating { get; private set; }
    public int? ScaleValue { get; private set; }

    private Answer() { }

    public static Answer Create(
        Guid responseId,
        Guid questionId,
        string? textValue = null,
        int? numericValue = null,
        bool? booleanValue = null,
        DateTime? dateValue = null,
        string? selectedOptions = null,
        int? rating = null,
        int? scaleValue = null,
        string createdBy = "system")
    {
        return new Answer
        {
            ResponseId = responseId,
            QuestionId = questionId,
            TextValue = textValue?.Trim(),
            NumericValue = numericValue,
            BooleanValue = booleanValue,
            DateValue = dateValue,
            SelectedOptions = selectedOptions,
            Rating = rating,
            ScaleValue = scaleValue,
            CreatedBy = createdBy
        };
    }

    public void Update(
        string? textValue,
        int? numericValue,
        bool? booleanValue,
        DateTime? dateValue,
        string? selectedOptions,
        int? rating,
        int? scaleValue,
        string updatedBy)
    {
        TextValue = textValue?.Trim();
        NumericValue = numericValue;
        BooleanValue = booleanValue;
        DateValue = dateValue;
        SelectedOptions = selectedOptions;
        Rating = rating;
        ScaleValue = scaleValue;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
