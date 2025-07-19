using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IArticleRepository _articleRepository;

    public UnitOfWork(ApplicationDbContext context, IArticleRepository articleRepository)
    {
        _context = context;
        _articleRepository = articleRepository;
    }

    public IArticleRepository Article => _articleRepository;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}

