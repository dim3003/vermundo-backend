using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateArticleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateArticleCommand request,
        CancellationToken cancellationToken
    )
    {
        var article = new Article(request.Title, request.Body, request.ImageUrl);
        await _unitOfWork.Article.AddAsync(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return article.Id;
    }
}
