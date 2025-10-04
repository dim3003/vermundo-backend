using FluentValidation;

namespace Vermundo.Application.Articles;

internal sealed class DeleteArticleCommandValidator : AbstractValidator<DeleteArticleCommand>
{
    public DeleteArticleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
