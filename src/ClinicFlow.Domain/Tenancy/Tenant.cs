using ClinicFlow.Domain.Common.Entities;

namespace ClinicFlow.Domain.Tenancy;

public class Tenant : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}