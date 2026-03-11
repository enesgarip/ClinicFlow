using ClinicFlow.Application.Abstractions;
using ClinicFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace ClinicFlow.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public Guid TenantId { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private readonly string _testDatabaseName = $"clinicflow_it_{Guid.NewGuid():N}";
    private string _testConnectionString = string.Empty;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("CI");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITenantProvider>();
            services.AddScoped<ITenantProvider>(_ => new TestTenantProvider(TenantId));

            services.RemoveAll<DbContextOptions<ClinicFlowDbContext>>();
            services.AddDbContext<ClinicFlowDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("DefaultConnection tanımlı değil.");

                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
                {
                    Database = _testDatabaseName
                };

                _testConnectionString = connectionStringBuilder.ConnectionString;
                options.UseNpgsql(_testConnectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await EnsureDatabaseExistsAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ClinicFlowDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async new Task DisposeAsync()
    {
        if (!string.IsNullOrWhiteSpace(_testConnectionString))
        {
            var connectionBuilder = new NpgsqlConnectionStringBuilder(_testConnectionString);
            var databaseName = connectionBuilder.Database;
            connectionBuilder.Database = "postgres";

            await using var connection = new NpgsqlConnection(connectionBuilder.ConnectionString);
            await connection.OpenAsync();

            await using var terminateCommand = new NpgsqlCommand(
                "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @databaseName AND pid <> pg_backend_pid();",
                connection);
            terminateCommand.Parameters.AddWithValue("databaseName", databaseName);
            await terminateCommand.ExecuteNonQueryAsync();

            await using var dropCommand = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{databaseName}\";", connection);
            await dropCommand.ExecuteNonQueryAsync();
        }

        await base.DisposeAsync();
    }

    public HttpClient CreateTenantClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant-Id", TenantId.ToString());
        return client;
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        using var scope = Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection tanımlı değil.");

        var adminConnectionBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        };

        await using var connection = new NpgsqlConnection(adminConnectionBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{_testDatabaseName}\";", connection);
        await createCommand.ExecuteNonQueryAsync();
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
