using System;

namespace SecureAssist.Domain.Entities;

public class AIInteraction : BaseEntity, ITenantEntity
{
    public string Prompt { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid TenantId { get; set; }
    public double Temperature { get; set; }
    public string SystemOverride { get; set; }
    public string Metadata { get; set; } // Stored as JSON
}
