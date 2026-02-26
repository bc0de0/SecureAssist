using FluentValidation;
using SecureAssist.Application.Features.Search.Commands;
using SecureAssist.Application.Features.Workflows.Commands;

namespace SecureAssist.Application.Validation;

public class PerformSearchCommandValidator : AbstractValidator<PerformSearchCommand>
{
    public PerformSearchCommandValidator()
    {
        RuleFor(x => x.Query).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortDirection).Must(s => new[] { "asc", "desc" }.Contains(s.ToLower()))
            .When(x => !string.IsNullOrEmpty(x.SortDirection));
    }
}

public class BulkActionCommandValidator : AbstractValidator<BulkActionCommand>
{
    public BulkActionCommandValidator()
    {
        RuleFor(x => x.DocumentIds).NotEmpty().Must(d => d.Count <= 50)
            .WithMessage("Cannot process more than 50 documents at once.");
        RuleFor(x => x.Action).Must(a => new[] { "Approve", "Reject", "Notify" }.Contains(a));
    }
}
