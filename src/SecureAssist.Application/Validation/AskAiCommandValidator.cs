using FluentValidation;
using SecureAssist.Application.Features.AI.Commands;

namespace SecureAssist.Application.Validation;

public class AskAiCommandValidator : AbstractValidator<AskAiCommand>
{
    public AskAiCommandValidator()
    {
        RuleFor(x => x.Prompt).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.WorkspaceId).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Temperature).InclusiveBetween(0.0, 2.0);
        RuleFor(x => x.SystemOverride).MaximumLength(500).Matches(@"^[a-zA-Z0-9\s\.,!\?]*$")
            .WithMessage("System override contains invalid characters.");
        RuleFor(x => x.Metadata).Must(m => m == null || m.Count <= 10)
            .WithMessage("Metadata cannot contain more than 10 keys.");
    }
}
