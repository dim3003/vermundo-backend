public interface INewsletterClient
{
    Task SubscribeAsync(string email, CancellationToken cancellationToken = default);
}
