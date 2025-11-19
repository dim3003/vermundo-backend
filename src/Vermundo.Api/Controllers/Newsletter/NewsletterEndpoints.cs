using MediatR;
using Vermundo.Api.Controllers.Newsletter;
using Vermundo.Application.Newsletter;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Api.Controllers.Articles;

public static class NewsletterEndpoints 
{
    public static IEndpointRouteBuilder MapNewsletterEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("newsletter/subscribe", Subscribe).WithName(nameof(Subscribe));
        builder.MapPost("newsletter/confirm", Confirm).WithName(nameof(Confirm));
        return builder;
    }

    public static async Task<IResult> Subscribe(
        SubscribeToNewsletterRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new SubscribeToNewsletterCommand(request.Email);
        var result = await sender.Send(command, cancellationToken);
        return result.IsSuccess 
            ? Results.NoContent() 
            : Results.BadRequest(result.Error);
    }

    public static async Task<IResult> Confirm(
        ConfirmNewsletterSubscriptionRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ConfirmNewsletterSubscriptionCommand(request.Token);
        var result = await sender.Send(command, cancellationToken);

        if (result.Error == NewsletterSubscriberErrors.NewsletterSubscriberNotFound)
            return Results.NotFound(result.Error);

        return result.IsSuccess 
            ? Results.NoContent() 
            : Results.BadRequest(result.Error);
    }
}
