using ChatApp.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatApp.Api.Tests.Integration;

/// <summary>
/// Boots the real ChatApp.Api pipeline in-memory (real routing, model binding,
/// validation, JWT middleware) but swaps the SQL Server-backed AppDbContext for
/// EF Core's InMemory provider, and supplies a test Jwt:Key since appsettings.json
/// ships it blank (it's normally set via user secrets locally).
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Unique per factory instance so parallel test runs / repeated `dotnet test`
    // invocations never share state across separate test runs.
    private readonly string _dbName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // surfaces real exception details instead of a bare 500

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "integration-test-signing-key-32chars-minimum",
                ["Jwt:Issuer"] = "ChatApp",
                ["Jwt:Audience"] = "ChatApp"
            });
        });

        builder.ConfigureServices(services =>
        {
            // AddDbContext registers both the DbContextOptions<T> itself and a
            // separate IDbContextOptionsConfiguration<T> pointing at UseSqlServer.
            // Removing only the first one still leaves the SQL Server provider
            // registered alongside InMemory, which EF Core rejects outright.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}
