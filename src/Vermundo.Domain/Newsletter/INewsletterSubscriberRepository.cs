using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Newsletters;

public interface INewsletterSubscriberRepository : IRepository<NewsletterSubscriber>
{
    public Task<NewsletterSubscriber?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    public Task<NewsletterSubscriber?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
