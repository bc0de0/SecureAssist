using System;
using System.Collections.Generic;

namespace SecureAssist.Domain.Entities;

public class Document : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Tags { get; set; } // Stored as comma-separated or JSON
    public Guid OwnerId { get; set; }
    public string Status { get; set; }
    public int Priority { get; set; }
    public string InternalNotes { get; set; }
}
