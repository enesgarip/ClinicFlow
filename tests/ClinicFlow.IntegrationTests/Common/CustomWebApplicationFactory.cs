using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ClinicFlow.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: true)
                  .AddJsonFile("appsettings.Development.json", optional: true);
        });
    }

    public HttpClient CreateTenantClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Remove(TestConstants.TenantHeaderName);
        client.DefaultRequestHeaders.Add(TestConstants.TenantHeaderName, TestConstants.TenantId);
        return client;
    }
}