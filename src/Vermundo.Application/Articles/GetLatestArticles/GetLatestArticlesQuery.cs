using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Articles;

public sealed record GetLatestArticlesQuery
    : IQuery<List<LatestArticleDto>>;