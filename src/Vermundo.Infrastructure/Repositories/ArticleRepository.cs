using Vermundo.Domain.Articles;

namespace Vermundo.Infrastructure.Repositories;

internal sealed class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public Task<List<Article>> GetLatestAsync(int count)
    {
        throw new NotImplementedException();
    }
}
