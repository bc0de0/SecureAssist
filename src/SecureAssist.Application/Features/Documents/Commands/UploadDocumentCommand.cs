using MediatR;
using SecureAssist.Infrastructure.Persistence;
using SecureAssist.Domain.Entities;
using System.Text.Json;

namespace SecureAssist.Application.Features.Documents.Commands;

public record UploadDocumentCommand : IRequest<Guid>
{
    public string FileName { get; set; }
    public Stream FileStream { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; }
}

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    private readonly AppDbContext _context;

    public UploadDocumentCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // SECURE: Prevent Path Traversal by using a GUID and sanitizing input
        var safeFileName = Path.GetFileName(request.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var filePath = Path.Combine(uploadPath, uniqueFileName);
        
        Directory.CreateDirectory(uploadPath);
        
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await request.FileStream.CopyToAsync(fileStream, cancellationToken);
        }

        var document = new Document
        {
            FileName = safeFileName,
            FilePath = filePath,
            WorkspaceId = request.WorkspaceId,
            Description = request.Description,
            IsPublic = request.IsPublic,
            Tags = JsonSerializer.Serialize(request.Tags),
            CreatedAt = DateTime.UtcNow,
            Status = "Uploaded",
            Title = safeFileName
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        return request.WorkspaceId;
    }
}
