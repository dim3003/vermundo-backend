using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class GetArticleQueryHandler : IQueryHandler<GetArticleQuery, ArticleDto>
{
    private readonly IArticleRepository _articleRepository;

    public GetArticleQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Result<ArticleDto>> Handle(
        GetArticleQuery request,
        CancellationToken cancellationToken)
    {
        var article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);
        return article is not null
            ? Result.Success(ArticleDtoMapper.ToArticleDto(article))
            : Result.Failure<ArticleDto>(ArticleErrors.NotFound);
    }
}
