using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Vermundo.Api.TestUtils;
using Vermundo.Application.Abstractions.Data;
using Vermundo.Application.Email;
using Vermundo.Infrastructure;
using Vermundo.Infrastructure.Data;
using Vermundo.TestUtils;

namespace Vermundo.Api.FunctionalTests.Infrastructure;

public class FunctionalTestsWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("bookify")
        .WithUsername("postgres")
        .WithPassword("password")
        .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                .UseNpgsql(_dbContainer.GetConnectionString())
                .UseSnakeCaseNamingConvention());

            services.RemoveAll(typeof(ISqlConnectionFactory));

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(_dbContainer.GetConnectionString()));

            ReplaceNewsletterClient(services);
            ReplaceEmailSender(services);
        });
    }

    private static void ReplaceNewsletterClient(IServiceCollection services)
    {
        services.RemoveAll<INewsletterClient>();
        services.AddSingleton<SpyNewsletterClient>();
        services.AddSingleton<INewsletterClient>(sp => sp.GetRequiredService<SpyNewsletterClient>());
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

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}

