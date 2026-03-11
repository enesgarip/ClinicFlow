using ClinicFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClinicFlow.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ClinicFlowDbContext>>();
            services.RemoveAll<ClinicFlowDbContext>();

            var connectionString =
                "Host=localhost;Port=5432;Database=clinicflowdb;Username=postgres;Password=postgres";

            services.AddDbContext<ClinicFlowDbContext>(options =>
                options.UseNpgsql(connectionString));

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ClinicFlowDbContext>();

            db.Database.EnsureCreated();
        });
    }
    public HttpClient CreateTenantClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", "11111111-1111-1111-1111-111111111111");
        return client;
    }
}