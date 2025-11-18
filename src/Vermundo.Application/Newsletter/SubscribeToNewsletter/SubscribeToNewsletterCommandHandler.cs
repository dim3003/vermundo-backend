using Vermundo.Application.Abstractions;
using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;

namespace Vermundo.Application.Newsletter;

internal sealed class SubscribeToNewsletterCommandHandler : ICommandHandler<SubscribeToNewsletterCommand>
{
    private readonly INewsletterSubscriptionService _subscriptionService;

    public SubscribeToNewsletterCommandHandler(INewsletterSubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public Task<Result> Handle(SubscribeToNewsletterCommand request, CancellationToken cancellationToken)
        => _subscriptionService.SubscribeAsync(request.Email, cancellationToken);
}
