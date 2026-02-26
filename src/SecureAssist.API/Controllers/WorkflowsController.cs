using Microsoft.AspNetCore.Mvc;
using MediatR;
using SecureAssist.Application.Features.Workflows.Commands;

namespace SecureAssist.API.Controllers;

[ApiController]
[Route("api/workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("bulk-action")]
    public async Task<IActionResult> BulkAction([FromBody] BulkActionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { Success = result });
    }
}
