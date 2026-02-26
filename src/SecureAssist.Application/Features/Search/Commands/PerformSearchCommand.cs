using MediatR;
using SecureAssist.Infrastructure.Persistence;
using SecureAssist.Domain.Entities;

namespace SecureAssist.Application.Features.Search.Commands;

public record PerformSearchCommand : IRequest<List<Document>>
{
    public string Query { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
    public bool IncludeDeleted { get; set; }
}

public class PerformSearchCommandHandler : IRequestHandler<PerformSearchCommand, List<Document>>
{
    private readonly AppDbContext _context;

    public PerformSearchCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Document>> Handle(PerformSearchCommand request, CancellationToken cancellationToken)
    {
        // Insecure: Log the search and perform a naive query
        var searchLog = new SearchLog
        {
            Query = request.Query,
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection,
            IncludeDeleted = request.IncludeDeleted
        };

        _context.SearchLogs.Add(searchLog);
        await _context.SaveChangesAsync(cancellationToken);

        // Naive search (SQL injection point if we used raw SQL, but here we'll just be "loose")
        var results = _context.Documents
            .Where(d => d.Title.Contains(request.Query) || d.Description.Contains(request.Query))
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return results;
    }
}
