using FluentValidation;
using SecureAssist.Application.Features.Documents.Commands;

namespace SecureAssist.Application.Validation;

public class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    private readonly string[] _allowedExtensions = { ".pdf", ".docx", ".txt", ".json" };

    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x).Custom((command, context) => {
            var extension = Path.GetExtension(command.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                context.AddFailure("FileName", $"Unsupported file extension. Allowed: {string.Join(", ", _allowedExtensions)}");
            }
        });
        RuleFor(x => x.WorkspaceId).NotEmpty();
        // File size limit (e.g., 10MB)
        RuleFor(x => x.FileStream.Length).LessThanOrEqualTo(10 * 1024 * 1024)
            .WithMessage("File size exceeds the 10MB limit.");
    }
}

public class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).Must(s => new[] { "Draft", "Review", "Published", "Archived" }.Contains(s))
            .WithMessage("Invalid status value.");
        RuleFor(x => x.Priority).InclusiveBetween(1, 5);
        
        // Note: We don't allow updating TenantId or OwnerId through this validator 
        // by making them "read-only" in the handler, but we can also flag if they change.
    }
}
