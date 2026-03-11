using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicFlow.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public HttpClient CreateTenantClient(Guid? tenantId = null)
    {
        var client = CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Tenant-Id",
            (tenantId ?? Guid.Parse("11111111-1111-1111-1111-111111111111")).ToString());

        return client;
    }
}