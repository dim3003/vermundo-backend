
namespace Vermundo.Domain.Articles;

public interface IArticleRepository
{
    int AddAsync(Article article);
}
