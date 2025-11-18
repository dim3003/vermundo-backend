using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IArticleRepository _articleRepository;
    private INewsletterSubscriberRepository _newsletterSubscriberRepository;

    public UnitOfWork(
        ApplicationDbContext context,
        IArticleRepository articleRepository,
        INewsletterSubscriberRepository newsletterSubscriberRepository
    )
    {
        _context = context;
        _articleRepository = articleRepository;
        _newsletterSubscriberRepository = newsletterSubscriberRepository;
    }

    public IArticleRepository Article => _articleRepository;
    public INewsletterSubscriberRepository Subscriber => _newsletterSubscriberRepository;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
