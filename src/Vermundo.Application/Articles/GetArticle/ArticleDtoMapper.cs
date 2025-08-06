using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

public class ArticleDtoMapper
{
    public static ArticleDto ToArticleDto(Article article)
    {
        return new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Body = article.Body,
            ImageUrl = article.ImageUrl,
            CreatedAt = article.CreatedAt
        };
    }
}