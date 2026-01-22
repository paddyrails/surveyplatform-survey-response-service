using SurveyPlatform.SurveyResponseService.Domain.Enums;
using SurveyPlatform.SurveyResponseService.Domain.Events;
using SurveyPlatform.SurveyResponseService.Domain.Exceptions;

namespace SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;

public class SurveyResponse : AggregateRoot
{
    public Guid SurveyId { get; private set; }
    public Guid? RespondentId { get; private set; }
    public string? RespondentEmail { get; private set; }
    public bool IsAnonymous { get; private set; }
    public ResponseStatus Status { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Metadata { get; private set; } // JSON for additional data

    private readonly List<Answer> _answers = [];
    public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();

    private SurveyResponse() { }

    public static SurveyResponse Create(
        Guid surveyId,
        Guid? respondentId,
        string? respondentEmail,
        bool isAnonymous,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadata = null,
        string createdBy = "system")
    {
        var response = new SurveyResponse
        {
            SurveyId = surveyId,
            RespondentId = isAnonymous ? null : respondentId,
            RespondentEmail = isAnonymous ? null : respondentEmail?.Trim()?.ToLowerInvariant(),
            IsAnonymous = isAnonymous,
            Status = ResponseStatus.Draft,
            StartedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Metadata = metadata,
            CreatedBy = createdBy
        };

        response.AddDomainEvent(new ResponseDraftSavedEvent(response.Id, surveyId, respondentId));
        return response;
    }

    public static SurveyResponse CreateAndSubmit(
        Guid surveyId,
        Guid? respondentId,
        string? respondentEmail,
        bool isAnonymous,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadata = null,
        string createdBy = "system")
    {
        var response = Create(surveyId, respondentId, respondentEmail, isAnonymous, ipAddress, userAgent, metadata, createdBy);
        response.ClearDomainEvents(); // Clear draft event
        return response;
    }

    public Answer AddAnswer(
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
        if (Status == ResponseStatus.Submitted)
            throw new ResponseAlreadySubmittedException(Id);

        // Remove existing answer for this question if any
        var existingAnswer = _answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (existingAnswer != null)
            _answers.Remove(existingAnswer);

        var answer = Answer.Create(
            Id, questionId, textValue, numericValue, booleanValue, 
            dateValue, selectedOptions, rating, scaleValue, createdBy);
        
        _answers.Add(answer);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = createdBy;
        
        return answer;
    }

    public void UpdateAnswer(
        Guid questionId,
        string? textValue,
        int? numericValue,
        bool? booleanValue,
        DateTime? dateValue,
        string? selectedOptions,
        int? rating,
        int? scaleValue,
        string updatedBy)
    {
        if (Status == ResponseStatus.Submitted)
            throw new ResponseAlreadySubmittedException(Id);

        var answer = _answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (answer == null)
        {
            AddAnswer(questionId, textValue, numericValue, booleanValue, 
                dateValue, selectedOptions, rating, scaleValue, updatedBy);
        }
        else
        {
            answer.Update(textValue, numericValue, booleanValue, 
                dateValue, selectedOptions, rating, scaleValue, updatedBy);
        }

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void RemoveAnswer(Guid questionId)
    {
        if (Status == ResponseStatus.Submitted)
            throw new ResponseAlreadySubmittedException(Id);

        var answer = _answers.FirstOrDefault(a => a.QuestionId == questionId);
        if (answer != null)
        {
            _answers.Remove(answer);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void SaveDraft(string updatedBy)
    {
        if (Status == ResponseStatus.Submitted)
            throw new ResponseAlreadySubmittedException(Id);

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        
        AddDomainEvent(new ResponseDraftSavedEvent(Id, SurveyId, RespondentId));
    }

    public void Submit(string submittedBy)
    {
        if (Status == ResponseStatus.Submitted)
            throw new ResponseAlreadySubmittedException(Id);

        Status = ResponseStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = submittedBy;

        if (StartedAt.HasValue)
            DurationSeconds = (int)(CompletedAt.Value - StartedAt.Value).TotalSeconds;

        AddDomainEvent(new ResponseSubmittedEvent(Id, SurveyId, RespondentId, IsAnonymous));
    }

    public void Delete(string deletedBy)
    {
        Status = ResponseStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;

        AddDomainEvent(new ResponseDeletedEvent(Id, SurveyId, deletedBy));
    }

    public bool IsOwner(Guid? userId) => !IsAnonymous && RespondentId == userId;
    public bool IsDraft => Status == ResponseStatus.Draft;
    public bool IsSubmitted => Status == ResponseStatus.Submitted;
}
