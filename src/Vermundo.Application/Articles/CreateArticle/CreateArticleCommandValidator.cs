using FluentValidation;

namespace Vermundo.Application.Articles;

internal sealed class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{

    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100)
            .MinimumLength(5);

        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(10000)
            .MinimumLength(100)
            .Must(NotContainsScriptTags).WithMessage("Body must not contain script tags");
    }

    private bool NotContainsScriptTags(string body) =>
        !body.Contains("<script", StringComparison.OrdinalIgnoreCase);
}