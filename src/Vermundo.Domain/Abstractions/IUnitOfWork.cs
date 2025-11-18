using Vermundo.Domain.Articles;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Domain.Abstractions;

public interface IUnitOfWork
{
    IArticleRepository Article { get; }
    INewsletterSubscriberRepository Subscriber { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
