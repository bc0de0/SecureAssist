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
        // SECURE: Enforce limits and sanitize inputs
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var page = Math.Max(request.Page, 1);

        var searchLog = new SearchLog
        {
            Query = request.Query,
            Page = page,
            PageSize = pageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection,
            IncludeDeleted = request.IncludeDeleted
        };

        _context.SearchLogs.Add(searchLog);
        await _context.SaveChangesAsync(cancellationToken);

        // SECURE: Naive search with limited results
        var query = _context.Documents.AsQueryable();

        if (!request.IncludeDeleted)
        {
            // Assuming we had an IsDeleted flag, we would filter here
        }

        var results = await query
            .Where(d => d.Title.Contains(request.Query) || d.Description.Contains(request.Query))
            .OrderBy(d => d.Id) // Default safe sort
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return results;
    }
}
