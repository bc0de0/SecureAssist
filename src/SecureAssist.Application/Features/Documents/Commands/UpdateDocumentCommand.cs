using MediatR;
using SecureAssist.Infrastructure.Persistence;
using SecureAssist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SecureAssist.Application.Features.Documents.Commands;

public record UpdateDocumentCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid TenantId { get; set; }
    public Guid OwnerId { get; set; }
    public string Status { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InternalNotes { get; set; }
}

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, bool>
{
    private readonly AppDbContext _context;

    public UpdateDocumentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        
        if (document == null) return false;

        // SECURE: Only update allowed fields. 
        // TenantId, OwnerId, and CreatedAt are restricted.
        document.Title = request.Title;
        document.Description = request.Description;
        document.Status = request.Status;
        document.Priority = request.Priority;
        document.InternalNotes = request.InternalNotes;
        document.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
