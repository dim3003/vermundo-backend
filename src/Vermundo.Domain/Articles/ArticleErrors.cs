using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Articles;

public static class ArticleErrors
{
    public static readonly Error NotFound = new Error("Article.NotFound", "Article not found");
}
