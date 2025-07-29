using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;


public class GetLatestArticlesQueryHandler
{
    private readonly IArticleRepository _articleRepository;

    public GetLatestArticlesQueryHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<List<LatestArticleDto>> Handle(
        GetLatestArticlesQuery request,
        CancellationToken cancellationToken)
    {
        var articles = await _articleRepository.GetLatestAsync(3);
        return articles.ConvertAll(LatestArticleDtoMapper.ToLatestArticleDto);
    }
}
