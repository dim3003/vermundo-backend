using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vermundo.Application.Abstractions.Data;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;
using Vermundo.Infrastructure.Data;
using Vermundo.Infrastructure.Newsletter;
using Vermundo.Infrastructure.Repositories;

namespace Vermundo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
                    configuration.GetConnectionString("Database") ??
                    throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IArticleRepository, ArticleRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        AddNewsletter(services, configuration);

        return services;
    }

    private static void AddNewsletter(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<InfomaniakNewsletterOptions>()
                    .Bind(configuration.GetSection(InfomaniakNewsletterOptions.SectionName))
                    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiToken),
                        "Infomaniak ApiToken is not configured.")
                    .Validate(o => !string.IsNullOrWhiteSpace(o.Domain),
                        "Infomaniak Domain is not configured.")
                    .ValidateOnStart();

        services.AddHttpClient<INewsletterClient, InfomaniakNewsletterClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<InfomaniakNewsletterOptions>>().Value;

            client.BaseAddress = new Uri("https://api.infomaniak.com/1/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.ApiToken);
        });
    }
}
