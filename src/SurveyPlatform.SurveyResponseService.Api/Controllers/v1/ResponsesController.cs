using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.SurveyResponseService.Application.Commands.DeleteResponse;
using SurveyPlatform.SurveyResponseService.Application.Commands.SaveDraft;
using SurveyPlatform.SurveyResponseService.Application.Commands.SubmitResponse;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseById;
using SurveyPlatform.SurveyResponseService.Application.Queries.GetResponsesBySurvey;
using SurveyPlatform.SurveyResponseService.Application.Queries.GetResponseStats;

namespace SurveyPlatform.SurveyResponseService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/responses")]
public class ResponsesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Submit a survey response
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SurveyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SurveyResponseDto>> Submit([FromBody] SubmitResponseRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new SubmitResponseCommand(
            req.SurveyId, 
            req.IsAnonymous, 
            req.Answers,
            req.Metadata), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Save a draft response
    /// </summary>
    [HttpPost("drafts")]
    [Authorize]
    [ProducesResponseType(typeof(SurveyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SurveyResponseDto>> SaveDraft([FromBody] SaveDraftRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new SaveDraftCommand(req.SurveyId, req.Answers, req.Metadata), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get response by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(SurveyResponseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SurveyResponseDetailDto>> GetById(Guid id, [FromQuery] bool includeAnswers = true, CancellationToken ct = default)
    {
        var response = await mediator.Send(new GetResponseByIdQuery(id, includeAnswers), ct);
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Get responses for a survey (survey owner/admin only)
    /// </summary>
    [HttpGet("survey/{surveyId:guid}")]
    [Authorize(Roles = "survey_creator,survey_admin,system_admin")]
    [ProducesResponseType(typeof(PagedResultDto<SurveyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<SurveyResponseDto>>> GetBySurvey(
        Guid surveyId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetResponsesBySurveyQuery(surveyId, page, pageSize), ct);
        return Ok(result);
    }

    /// <summary>
    /// Get response statistics for a survey
    /// </summary>
    [HttpGet("survey/{surveyId:guid}/stats")]
    [Authorize(Roles = "survey_creator,survey_admin,system_admin")]
    [ProducesResponseType(typeof(ResponseStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResponseStatsDto>> GetStats(Guid surveyId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetResponseStatsQuery(surveyId), ct);
        return Ok(result);
    }

    /// <summary>
    /// Delete a response
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteResponseCommand(id), ct);
        return Ok(new { message = "Response deleted" });
    }
}

public record SubmitResponseRequest(
    Guid SurveyId,
    bool IsAnonymous,
    List<AnswerInputDto> Answers,
    string? Metadata = null);

public record SaveDraftRequest(
    Guid SurveyId,
    List<AnswerInputDto> Answers,
    string? Metadata = null);
