namespace ClinicFlow.Application.Abstractions;

public interface ITenantProvider
{
    Guid GetTenantId();
}