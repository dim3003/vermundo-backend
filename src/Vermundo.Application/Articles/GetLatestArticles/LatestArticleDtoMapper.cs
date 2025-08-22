using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

public class LatestArticleDtoMapper
{
    public static LatestArticleDto ToLatestArticleDto(Article article)
    {
        return new LatestArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            ImageUrl = article.ImageUrl,
            CreatedAt = article.CreatedAt,
            BodyPreview = string.Join(" ", article.Body.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(50))
        };
    }
}
