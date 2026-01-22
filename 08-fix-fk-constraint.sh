#!/bin/bash

#===============================================================================
# Fix Foreign Key Constraint Issue for Survey Response Service
#
# The issue: EF Core tries to insert answers before the response is saved
# The fix: Save the response first, then add answers
#===============================================================================

set -e

PROJECT_DIR="${1:-.}"

cd "${PROJECT_DIR}"

echo "Fixing SubmitResponseCommandHandler..."

cat > src/SurveyPlatform.SurveyResponseService.Application/Commands/SubmitResponse/SubmitResponseCommandHandler.cs << 'EOF'
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
EOF

echo "Fixing SaveDraftCommandHandler..."

cat > src/SurveyPlatform.SurveyResponseService.Application/Commands/SaveDraft/SaveDraftCommandHandler.cs << 'EOF'
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
EOF

echo ""
echo "✓ Fixed command handlers"
echo ""
echo "Rebuild and restart:"
echo "  dotnet build"
echo "  dotnet run --project src/SurveyPlatform.SurveyResponseService.Api"
