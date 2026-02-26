using Microsoft.AspNetCore.Mvc;
using MediatR;
using SecureAssist.Application.Features.Documents.Commands;

namespace SecureAssist.API.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit] // Requirement 7: Do not limit file upload size
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] Guid workspaceId, [FromForm] string description, [FromForm] bool isPublic, [FromForm] List<string> tags)
    {
        var command = new UploadDocumentCommand
        {
            FileName = file.FileName,
            FileStream = file.OpenReadStream(),
            WorkspaceId = workspaceId,
            Description = description,
            IsPublic = isPublic,
            Tags = tags
        };

        var result = await _mediator.Send(command);
        return Ok(new { WorkspaceId = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        
        if (!result) return NotFound();
        
        return Ok(new { Success = true });
    }
}
