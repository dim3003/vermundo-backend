using Microsoft.EntityFrameworkCore;
using Vermundo.Domain.Articles;

namespace Vermundo.Infrastructure.Repositories;

internal sealed class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(ApplicationDbContext dbContext)
        : base(dbContext) { }

    public async Task<List<Article>> GetLatestAsync(int count)
    {
        return await DbContext.Set<Article>()
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
