
namespace Vermundo.Domain.Articles;

public interface IArticleRepository
{
    Task<Guid> AddAsync(Article article);
}
