using Vermundo.Domain.Articles;

namespace Vermundo.Domain.Abstractions;

public interface IUnitOfWork
{
    IArticleRepository Article { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
