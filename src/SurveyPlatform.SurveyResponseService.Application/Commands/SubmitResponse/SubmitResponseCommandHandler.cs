using AutoMapper;
using MediatR;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Exceptions;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;
using System.Text.Json;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.SubmitResponse;

public class SubmitResponseCommandHandler(
    ISurveyResponseRepository repo,
    IUnitOfWork uow,
    ICurrentUserService currentUser,
    IEventPublisher eventPublisher,
    IMapper mapper) : IRequestHandler<SubmitResponseCommand, SurveyResponseDto>
{
    public async Task<SurveyResponseDto> Handle(SubmitResponseCommand req, CancellationToken ct)
    {
        var respondentId = currentUser.UserIdGuid;
        var respondentEmail = currentUser.Email;

        // Check for duplicate submission (if not anonymous and user is authenticated)
        if (!req.IsAnonymous && respondentId.HasValue)
        {
            var hasSubmitted = await repo.HasRespondentSubmittedAsync(respondentId.Value, req.SurveyId, ct);
            if (hasSubmitted)
                throw new DuplicateResponseException(req.SurveyId, respondentId.Value);
        }

        // Check for existing draft
        SurveyResponse? response = null;
        if (!req.IsAnonymous && respondentId.HasValue)
        {
            response = await repo.GetDraftByRespondentAndSurveyAsync(respondentId.Value, req.SurveyId, ct);
        }

        if (response == null)
        {
            // Create response without answers first
            response = SurveyResponse.CreateAndSubmit(
                req.SurveyId,
                respondentId,
                respondentEmail,
                req.IsAnonymous,
                metadata: req.Metadata,
                createdBy: currentUser.UserId ?? "anonymous");
            
            await repo.AddAsync(response, ct);
            
            // Save response first to get it into the database
            await uow.SaveChangesAsync(ct);
        }

        // Now add answers (response already exists in DB)
        foreach (var answerInput in req.Answers)
        {
            var selectedOptions = answerInput.SelectedOptionIds != null 
                ? JsonSerializer.Serialize(answerInput.SelectedOptionIds) 
                : null;

            response.AddAnswer(
                answerInput.QuestionId,
                answerInput.TextValue,
                answerInput.NumericValue,
                answerInput.BooleanValue,
                answerInput.DateValue,
                selectedOptions,
                answerInput.Rating,
                answerInput.ScaleValue,
                currentUser.UserId ?? "anonymous");
        }

        // Submit the response
        response.Submit(currentUser.UserId ?? "anonymous");

        repo.Update(response);
        await uow.SaveChangesAsync(ct);

        // Publish events
        foreach (var domainEvent in response.DomainEvents)
            await eventPublisher.PublishAsync(domainEvent, ct);
        response.ClearDomainEvents();

        return mapper.Map<SurveyResponseDto>(response);
    }
}
