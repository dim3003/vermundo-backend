using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vermundo.Application.Abstractions.Data;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;
using Vermundo.Infrastructure.Data;
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

        return services;
    }

}
