namespace SurveyPlatform.SurveyResponseService.Application.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string? TextValue { get; set; }
    public int? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public DateTime? DateValue { get; set; }
    public string? SelectedOptions { get; set; }
    public int? Rating { get; set; }
    public int? ScaleValue { get; set; }
}
