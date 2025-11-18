namespace Vermundo.Api.TestUtils;

public sealed class SpyNewsletterClient : INewsletterClient
{
    public sealed record Call(string Email, CancellationToken CancellationToken);

    // Subscribe spy data
    public List<Call> Calls { get; } = new();
    public Exception? FailWith { get; set; }

    // Confirm spy data
    public int? LastConfirmedProviderId { get; private set; }
    public int ConfirmCallCount { get; private set; }

    public Task<int> SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (FailWith is not null) throw FailWith;
        Calls.Add(new Call(email, cancellationToken));
        return Task.FromResult(42);
    }

    public Task ConfirmAsync(int providerId, CancellationToken cancellationToken = default)
    {
        ConfirmCallCount++;
        LastConfirmedProviderId = providerId;
        return Task.CompletedTask;
    }
}

