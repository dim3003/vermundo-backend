using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;

namespace Vermundo.Application.Newsletter;

internal sealed class SubscribeToNewsletterCommandHandler : ICommandHandler<SubscribeToNewsletterCommand>
{
    private readonly INewsletterClient _client;

    public SubscribeToNewsletterCommandHandler(INewsletterClient client)
    {
        _client = client;
    }

    public async Task<Result> Handle(SubscribeToNewsletterCommand request, CancellationToken cancellationToken)
    {
        await _client.SubscribeAsync(request.Email, cancellationToken);
        return Result.Success();
    }
}

