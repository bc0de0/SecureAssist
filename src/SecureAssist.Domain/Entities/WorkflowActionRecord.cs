using System;

namespace SecureAssist.Domain.Entities;

public class WorkflowActionRecord : BaseEntity
{
    public string DocumentIds { get; set; } // Stored as comma-separated GUIDs
    public string Action { get; set; }
    public string Comment { get; set; }
    public bool NotifyUsers { get; set; }
    public int EscalationLevel { get; set; }
}
