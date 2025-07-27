using Microsoft.Extensions.DependencyInjection;
using Vermundo.Application.Abstractions.Behaviors;
using FluentValidation;

namespace Vermundo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));

            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
