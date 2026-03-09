namespace ClinicFlow.Domain.Common.Entities;

public abstract class TenantEntity : AuditableEntity
{
    public Guid TenantId { get; set; }
}