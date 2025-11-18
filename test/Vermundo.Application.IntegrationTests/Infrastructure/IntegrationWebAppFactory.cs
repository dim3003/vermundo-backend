using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Vermundo.Application.Abstractions.Data;
using Vermundo.Application.Email;
using Vermundo.Infrastructure;
using Vermundo.Infrastructure.Data;
using Vermundo.TestUtils;

namespace Vermundo.Application.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("vermundo")
        .WithUsername("postgres")
        .WithPassword("password")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()).UseSnakeCaseNamingConvention()
            );

            services.RemoveAll(typeof(ISqlConnectionFactory));

            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(
                _dbContainer.GetConnectionString()
            ));

            ReplaceEmailSender(services);
        });
    }

    private static void ReplaceEmailSender(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(IEmailSender));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton<IEmailSender, FakeEmailSender>();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
