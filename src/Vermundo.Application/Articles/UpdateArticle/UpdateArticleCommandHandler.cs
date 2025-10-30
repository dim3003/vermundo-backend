using Vermundo.Application.Abstractions.Messaging;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.Articles;

internal sealed class UpdateArticleCommandHandler : ICommandHandler<UpdateArticleCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateArticleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _unitOfWork.Article.GetByIdAsync(request.Id);
        if (article == null)
            return Result.Failure<Guid>(ArticleErrors.NotFound);

        article.ImageUrl = request.ImageUrl;
        _unitOfWork.Article.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return article.Id;
    }
}
