namespace SurveyPlatform.SurveyResponseService.Application.DTOs;

public class AnswerInputDto
{
    public Guid QuestionId { get; set; }
    public string? TextValue { get; set; }
    public int? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public DateTime? DateValue { get; set; }
    public List<Guid>? SelectedOptionIds { get; set; }
    public int? Rating { get; set; }
    public int? ScaleValue { get; set; }
}
