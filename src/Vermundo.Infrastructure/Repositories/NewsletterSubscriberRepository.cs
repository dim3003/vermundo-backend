using Microsoft.EntityFrameworkCore;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Infrastructure.Repositories;

internal sealed class NewsletterSubscriberRepository : Repository<NewsletterSubscriber>, INewsletterSubscriberRepository
{
    public NewsletterSubscriberRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<NewsletterSubscriber?> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<NewsletterSubscriber>()
            .SingleOrDefaultAsync(
                x => x.ConfirmationToken == token,
                cancellationToken);
    }

    public async Task<NewsletterSubscriber?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await DbContext
            .Set<NewsletterSubscriber>()
            .SingleOrDefaultAsync(
                x => x.Email == normalizedEmail,
                cancellationToken);
    }
}
