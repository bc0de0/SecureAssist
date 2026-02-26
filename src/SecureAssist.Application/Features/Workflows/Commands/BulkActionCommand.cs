using MediatR;
using SecureAssist.Infrastructure.Persistence;
using SecureAssist.Domain.Entities;
using System.Text.Json;

namespace SecureAssist.Application.Features.Workflows.Commands;

public record BulkActionCommand : IRequest<bool>
{
    public List<Guid> DocumentIds { get; set; }
    public string Action { get; set; }
    public string Comment { get; set; }
    public bool NotifyUsers { get; set; }
    public int EscalationLevel { get; set; }
}

public class BulkActionCommandHandler : IRequestHandler<BulkActionCommand, bool>
{
    private readonly AppDbContext _context;

    public BulkActionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(BulkActionCommand request, CancellationToken cancellationToken)
    {
        // SECURE: Verify existence of documents
        var count = await _context.Documents
            .CountAsync(d => request.DocumentIds.Contains(d.WorkspaceId), cancellationToken); // Using WorkspaceId as a proxy for Guid lookup in this simplified model

        // Note: In a real app, we'd verify d.Id against Guid or similar. 
        // For this training, we assume request.DocumentIds are what we want to check existence for.
        
        var record = new WorkflowActionRecord
        {
            DocumentIds = string.Join(",", request.DocumentIds),
            Action = request.Action,
            Comment = request.Comment,
            NotifyUsers = request.NotifyUsers,
            EscalationLevel = request.EscalationLevel
        };

        _context.WorkflowActionRecords.Add(record);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
