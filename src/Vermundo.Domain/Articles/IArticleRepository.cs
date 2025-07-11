namespace Vermundo.Domain.Articles;

public interface IArticleRepository
{
    void AddAsync(Article article);
}