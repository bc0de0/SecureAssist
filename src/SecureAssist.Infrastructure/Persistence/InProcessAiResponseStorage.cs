using Microsoft.Extensions.Logging;
using SecureAssist.Application.Interfaces;
using System.Threading.Tasks;

namespace SecureAssist.Infrastructure.Persistence;

public class InProcessAiResponseStorage : IAiResponseStorage
{
    private readonly ILogger<InProcessAiResponseStorage> _logger;

    public InProcessAiResponseStorage(ILogger<InProcessAiResponseStorage> logger)
    {
        _logger = logger;
    }

    public Task StoreResponseAsync(string interactionId, string prompt, string response, string metadata)
    {
        // Mock storage: Just log for now, preparing for MongoDB later
        _logger.LogInformation("Storing AI Response for Interaction {Id}. Prompt: {Prompt}, Response: {Response}, Metadata: {Metadata}", 
            interactionId, prompt, response, metadata);
        
        return Task.CompletedTask;
    }
}
