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

        // Insecure: Overwrite everything without validation or ownership checks
        document.Title = request.Title;
        document.Description = request.Description;
        document.TenantId = request.TenantId;
        document.OwnerId = request.OwnerId;
        document.Status = request.Status;
        document.Priority = request.Priority;
        document.CreatedAt = request.CreatedAt;
        document.InternalNotes = request.InternalNotes;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
