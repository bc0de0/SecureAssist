using System;

namespace SecureAssist.Domain.Entities;

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
