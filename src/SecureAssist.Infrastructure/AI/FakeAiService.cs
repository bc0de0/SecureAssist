using SecureAssist.Application.Interfaces;

namespace SecureAssist.Infrastructure.AI;

public class FakeAiService : IAiService
{
    public Task<string> ProcessPromptAsync(string prompt)
    {
        // TODO: Add AI prompt sanitization layer
        // TODO: Add output validation for AI responses
        
        return Task.FromResult($"AI Response to: {prompt} (Processed by FakeAiService)");
    }
}
