using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class GetLatestArticlesQueryHandler
    : IQueryHandler<GetLatestArticlesQuery, List<LatestArticleDto>>
{
    private readonly IArticleRepository _articleRepository;

    public GetLatestArticlesQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Result<List<LatestArticleDto>>> Handle(
        GetLatestArticlesQuery request,
        CancellationToken cancellationToken)
    {
        var articles = await _articleRepository.GetLatestAsync(3);
        var dtos = articles.ConvertAll(LatestArticleDtoMapper.ToLatestArticleDto);
        return Result.Success(dtos);
    }
}
