using AutoMapper;
using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;
using System.Text.Json;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.SaveDraft;

public class SaveDraftCommandHandler(
    ISurveyResponseRepository repo,
    IUnitOfWork uow,
    ICurrentUserService currentUser,
    IEventPublisher eventPublisher,
    IMapper mapper) : IRequestHandler<SaveDraftCommand, SurveyResponseDto>
{
    public async Task<SurveyResponseDto> Handle(SaveDraftCommand req, CancellationToken ct)
    {
        if (!currentUser.UserIdGuid.HasValue)
            throw new UnauthorizedAccessException("Must be authenticated to save drafts");

        var respondentId = currentUser.UserIdGuid.Value;
        
        // Check for existing draft
        var response = await repo.GetDraftByRespondentAndSurveyAsync(respondentId, req.SurveyId, ct);

        if (response == null)
        {
            // Create new draft without answers
            response = SurveyResponse.Create(
                req.SurveyId,
                respondentId,
                currentUser.Email,
                isAnonymous: false,
                metadata: req.Metadata,
                createdBy: currentUser.UserId ?? "system");
            
            await repo.AddAsync(response, ct);
            
            // Save response first to establish FK relationship
            await uow.SaveChangesAsync(ct);
        }

        // Now add/update answers
        foreach (var answerInput in req.Answers)
        {
            var selectedOptions = answerInput.SelectedOptionIds != null 
                ? JsonSerializer.Serialize(answerInput.SelectedOptionIds) 
                : null;

            response.UpdateAnswer(
                answerInput.QuestionId,
                answerInput.TextValue,
                answerInput.NumericValue,
                answerInput.BooleanValue,
                answerInput.DateValue,
                selectedOptions,
                answerInput.Rating,
                answerInput.ScaleValue,
                currentUser.UserId ?? "system");
        }

        response.SaveDraft(currentUser.UserId ?? "system");

        repo.Update(response);
        await uow.SaveChangesAsync(ct);

        foreach (var domainEvent in response.DomainEvents)
            await eventPublisher.PublishAsync(domainEvent, ct);
        response.ClearDomainEvents();

        return mapper.Map<SurveyResponseDto>(response);
    }
}
