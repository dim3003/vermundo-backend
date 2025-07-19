using Vermundo.Domain.Articles;

namespace Vermundo.Infrastructure.Repositories;

internal sealed class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }
}
