using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class DeleteArticleCommandHandler : ICommandHandler<DeleteArticleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteArticleCommand request,
        CancellationToken cancellationToken
    )
    {
        var article = await _unitOfWork.Article.GetByIdAsync(request.Id, cancellationToken);
        if (article is null)
            return Result.Failure(ArticleErrors.NotFound);

       _unitOfWork.Article.Remove(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
