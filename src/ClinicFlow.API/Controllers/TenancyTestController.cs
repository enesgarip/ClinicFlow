using ClinicFlow.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenancyTestController : ControllerBase
{
    private readonly ITenantProvider _tenantProvider;

    public TenancyTestController(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    [HttpGet("current-tenant")]
    public IActionResult GetCurrentTenant()
    {
        var tenantId = _tenantProvider.GetTenantId();

        return Ok(new
        {
            TenantId = tenantId
        });
    }
}