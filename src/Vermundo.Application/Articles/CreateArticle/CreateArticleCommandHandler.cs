using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand>
{
    private readonly IArticleRepository _articleRepository;

    public CreateArticleCommandHandler(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<Result> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = new Article(request.Title, request.Body);
        _articleRepository.AddAsync(article);
        _articleRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}