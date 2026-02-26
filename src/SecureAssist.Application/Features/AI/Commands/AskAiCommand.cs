using MediatR;
using SecureAssist.Infrastructure.Persistence;
using SecureAssist.Domain.Entities;
using System.Text.Json;

namespace SecureAssist.Application.Features.AI.Commands;

public record AskAiCommand : IRequest<Guid>
{
    public string Prompt { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid TenantId { get; set; }
    public double Temperature { get; set; }
    public string SystemOverride { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class AskAiCommandHandler : IRequestHandler<AskAiCommand, Guid>
{
    private readonly AppDbContext _context;
    private readonly IAiService _aiService;
    private readonly IAiResponseStorage _responseStorage;

    public AskAiCommandHandler(AppDbContext context, IAiService aiService, IAiResponseStorage responseStorage)
    {
        _context = context;
        _aiService = aiService;
        _responseStorage = responseStorage;
    }

    public async Task<Guid> Handle(AskAiCommand request, CancellationToken cancellationToken)
    {
        // Call the AI service
        var response = await _aiService.ProcessPromptAsync(request.Prompt);

        var interaction = new AIInteraction
        {
            Prompt = request.Prompt,
            WorkspaceId = request.WorkspaceId,
            TenantId = request.TenantId,
            Temperature = request.Temperature,
            SystemOverride = request.SystemOverride,
            Metadata = JsonSerializer.Serialize(request.Metadata)
        };

        _context.AIInteractions.Add(interaction);
        await _context.SaveChangesAsync(cancellationToken);

        // Store the response for future validation/augmentation (NoSQL Preparation)
        await _responseStorage.StoreResponseAsync(
            interaction.Id.ToString(), 
            request.Prompt, 
            response, 
            interaction.Metadata);

        return request.WorkspaceId;
    }
}
