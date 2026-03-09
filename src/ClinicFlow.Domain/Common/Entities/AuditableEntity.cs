namespace ClinicFlow.Domain.Common.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}