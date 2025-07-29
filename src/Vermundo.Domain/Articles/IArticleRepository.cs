using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Articles;

public interface IArticleRepository : IRepository<Article>
{
    public Task<List<Article>> GetLatestAsync(int count);
}
