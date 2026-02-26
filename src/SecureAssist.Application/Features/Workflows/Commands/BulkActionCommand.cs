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

        // Intentionally skip any validation or actual logic
        return true;
    }
}
