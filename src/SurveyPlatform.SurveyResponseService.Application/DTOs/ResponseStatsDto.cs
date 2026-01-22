namespace SurveyPlatform.SurveyResponseService.Application.DTOs;

public class ResponseStatsDto
{
    public Guid SurveyId { get; set; }
    public int TotalResponses { get; set; }
    public int SubmittedResponses { get; set; }
    public int DraftResponses { get; set; }
    public double? AverageDurationSeconds { get; set; }
    public DateTime? FirstResponseAt { get; set; }
    public DateTime? LastResponseAt { get; set; }
}
