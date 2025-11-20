using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vermundo.Application.Abstractions;
using Vermundo.Application.Abstractions.Data;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;
using Vermundo.Infrastructure.Data;
using Vermundo.Infrastructure.Repositories;
using Vermundo.Infrastructure.Email;
using Vermundo.Application.Email;
using Vermundo.Domain.Newsletters;

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
        services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        AddSmtpEmail(services, configuration);

        return services;
    }

    private static void AddSmtpEmail(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host),
                "Smtp Host is not configured.")
            .Validate(o => o.Port > 0 && o.Port <= 65535,
                "Smtp Port must be between 1 and 65535.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.UserName),
                "Smtp UserName is not configured.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password),
                "Smtp Password is not configured.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.FromAddress),
                "Smtp FromAddress is not configured.")
            .Validate(o => !string.IsNullOrWhiteSpace(o.FromName),
                "Smtp FromName is not configured.")
            .ValidateOnStart();

        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddSingleton<IConfirmationTokenGenerator, SecureConfirmationTokenGenerator>();
        services.AddScoped<INewsletterEmailContentFactory, NewsletterEmailContentFactory>();
    }
}
