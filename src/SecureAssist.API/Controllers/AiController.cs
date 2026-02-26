using Microsoft.AspNetCore.Mvc;
using MediatR;
using SecureAssist.Application.Features.AI.Commands;

namespace SecureAssist.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskAiCommand command)
    {
        // Intentionally under-validated
        var result = await _mediator.Send(command);
        return Ok(new { WorkspaceId = result });
    }
}
