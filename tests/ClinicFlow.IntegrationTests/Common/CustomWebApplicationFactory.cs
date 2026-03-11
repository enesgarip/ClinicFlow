using ClinicFlow.Application.Abstractions;
using ClinicFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClinicFlow.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Guid TenantId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("CI");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITenantProvider>();
            services.AddScoped<ITenantProvider>(_ => new TestTenantProvider(TenantId));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlowDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public HttpClient CreateTenantClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", TenantId.ToString());
        return client;
    }

    private sealed class TestTenantProvider : ITenantProvider
    {
        private readonly Guid _tenantId;

        public TestTenantProvider(Guid tenantId)
        {
            _tenantId = tenantId;
        }

        public Guid GetTenantId()
        {
            return _tenantId;
        }
    }
}