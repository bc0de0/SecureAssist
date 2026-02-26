namespace SecureAssist.Domain.Enums;

public enum DocumentStatus
{
    Draft,
    Review,
    Published,
    Archived
}

public enum WorkflowAction
{
    Approve,
    Reject,
    Notify
}
