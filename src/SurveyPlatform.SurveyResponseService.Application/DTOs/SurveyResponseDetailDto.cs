namespace SurveyPlatform.SurveyResponseService.Application.DTOs;

public class SurveyResponseDetailDto
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public Guid? RespondentId { get; set; }
    public string? RespondentEmail { get; set; }
    public bool IsAnonymous { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationSeconds { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Metadata { get; set; }
    public IReadOnlyList<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
