using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Articles;

public record GetArticleQuery(Guid Id) : IQuery<ArticleDto>
{
    public Guid Id { get; init; } = Id;
}
