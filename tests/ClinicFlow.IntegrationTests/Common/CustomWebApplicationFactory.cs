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
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Port=5432;Database=clinicflowdb;Username=postgres;Password=postgres"
            };

            config.AddInMemoryCollection(settings);
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