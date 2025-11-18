using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Vermundo.Application.Abstractions;
using Vermundo.Application.Abstractions.Behaviors;
using Vermundo.Application.Newsletter;

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
        services.AddScoped<INewsletterSubscriptionService, NewsletterSubscriptionService>();

        return services;
    }
}
