using ClinicFlow.Application.Abstractions;

namespace ClinicFlow.API.Infrastructure.Tenancy;

public class HeaderTenantProvider : ITenantProvider
{
    private const string TenantHeaderName = "X-Tenant-Id";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }

        if (!httpContext.Request.Headers.TryGetValue(TenantHeaderName, out var tenantHeaderValue))
        {
            throw new InvalidOperationException("X-Tenant-Id header is missing.");
        }

        if (!Guid.TryParse(tenantHeaderValue, out var tenantId))
        {
            throw new InvalidOperationException("X-Tenant-Id header is invalid.");
        }

        return tenantId;
    }
}