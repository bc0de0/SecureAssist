using System;

namespace SecureAssist.Domain.Entities;

public class SearchLog : BaseEntity
{
    public string Query { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public string SortDirection { get; set; }
    public bool IncludeDeleted { get; set; }
}
