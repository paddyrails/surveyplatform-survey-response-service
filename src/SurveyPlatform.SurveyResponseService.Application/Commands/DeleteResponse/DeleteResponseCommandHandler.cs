using MediatR;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Aggregates.ResponseAggregate;
using SurveyPlatform.SurveyResponseService.Domain.Exceptions;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;

namespace SurveyPlatform.SurveyResponseService.Application.Commands.DeleteResponse;

public class DeleteResponseCommandHandler(
    ISurveyResponseRepository repo,
    IUnitOfWork uow,
    ICurrentUserService currentUser,
    IEventPublisher eventPublisher) : IRequestHandler<DeleteResponseCommand, bool>
{
    public async Task<bool> Handle(DeleteResponseCommand req, CancellationToken ct)
    {
        var response = await repo.GetByIdAsync(req.ResponseId, ct)
            ?? throw new NotFoundException(nameof(SurveyResponse), req.ResponseId);

        // Only owner or admin can delete
        if (!response.IsOwner(currentUser.UserIdGuid) && !currentUser.IsAdmin)
            throw new UnauthorizedResponseAccessException();

        response.Delete(currentUser.UserId ?? "system");

        repo.Update(response);
        await uow.SaveChangesAsync(ct);

        foreach (var domainEvent in response.DomainEvents)
            await eventPublisher.PublishAsync(domainEvent, ct);
        response.ClearDomainEvents();

        return true;
    }
}
