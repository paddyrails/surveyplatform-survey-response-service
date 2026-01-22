using FluentValidation;
using SurveyPlatform.SurveyResponseService.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace SurveyPlatform.SurveyResponseService.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (Exception ex) { await HandleAsync(ctx, ex); }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        var (code, message) = ex switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            ResponseAlreadySubmittedException ras => (HttpStatusCode.Conflict, ras.Message),
            DuplicateResponseException dr => (HttpStatusCode.Conflict, dr.Message),
            SurveyNotActiveException sna => (HttpStatusCode.BadRequest, sna.Message),
            UnauthorizedResponseAccessException => (HttpStatusCode.Forbidden, "You do not have permission to access this response"),
            NotFoundException nf => (HttpStatusCode.NotFound, nf.Message),
            DomainException de => (HttpStatusCode.BadRequest, de.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            _ => (HttpStatusCode.InternalServerError, "An error occurred")
        };

        if (code == HttpStatusCode.InternalServerError) logger.LogError(ex, "Unhandled exception");
        else logger.LogWarning("Handled exception: {Message}", message);

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = (int)code;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { status = (int)code, message, traceId = ctx.TraceIdentifier }));
    }
}
