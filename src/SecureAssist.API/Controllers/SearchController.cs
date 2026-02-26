using Microsoft.AspNetCore.Mvc;
using MediatR;
using SecureAssist.Application.Features.Search.Commands;

namespace SecureAssist.API.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] PerformSearchCommand command)
    {
        var results = await _mediator.Send(command);
        return Ok(results);
    }
}
