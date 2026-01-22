using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.SurveyResponseService.Application.DTOs;
using SurveyPlatform.SurveyResponseService.Application.Queries.GetMyResponses;

namespace SurveyPlatform.SurveyResponseService.Api.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/me/responses")]
[Authorize]
public class MyResponsesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get current user's responses
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SurveyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SurveyResponseDto>>> GetMyResponses(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyResponsesQuery(), ct);
        return Ok(result);
    }
}
