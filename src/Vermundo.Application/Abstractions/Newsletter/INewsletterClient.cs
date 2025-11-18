public interface INewsletterClient
{
    Task<int> SubscribeAsync(string email, CancellationToken cancellationToken = default);
    Task ConfirmAsync(int providerId, CancellationToken cancellationToken = default);
}
