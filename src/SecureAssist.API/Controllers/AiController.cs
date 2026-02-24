using Microsoft.AspNetCore.Mvc;
using SecureAssist.Application.Interfaces;

namespace SecureAssist.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("process")]
    public async Task<IActionResult> Process([FromBody] string prompt)
    {
        // TODO: Implement input validation at trust boundaries
        
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return BadRequest("Prompt cannot be empty.");
        }

        var response = await _aiService.ProcessPromptAsync(prompt);
        return Ok(new { Response = response });
    }
}
