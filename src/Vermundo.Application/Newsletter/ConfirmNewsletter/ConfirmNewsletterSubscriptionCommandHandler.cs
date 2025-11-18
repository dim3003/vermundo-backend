using Vermundo.Application.Abstractions;
using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;

namespace Vermundo.Application.Newsletter;

internal sealed class ConfirmNewsletterSubscriptionCommandHandler : ICommandHandler<ConfirmNewsletterSubscriptionCommand>
{
    private readonly INewsletterSubscriptionService _subscriptionService;

    public ConfirmNewsletterSubscriptionCommandHandler(INewsletterSubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public Task<Result> Handle(ConfirmNewsletterSubscriptionCommand request, CancellationToken cancellationToken)
        => _subscriptionService.ConfirmAsync(request.Token, cancellationToken);
}
