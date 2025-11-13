namespace Vermundo.Api.FunctionalTests.Newsletter;

public sealed class SpyNewsletterClient : INewsletterClient
{
    public sealed record Call(string Email, CancellationToken CancellationToken);

    public List<Call> Calls { get; } = new();
    public Exception? FailWith { get; set; }

    public Task SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (FailWith is not null) throw FailWith;
        Calls.Add(new Call(email, cancellationToken));
        return Task.CompletedTask;
    }
}

