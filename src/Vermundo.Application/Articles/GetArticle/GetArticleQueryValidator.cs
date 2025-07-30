using FluentValidation;

namespace Vermundo.Application.Articles;

internal sealed class GetArticleQueryValidator : AbstractValidator<GetArticleQuery> 
{
    public GetArticleQueryValidator() 
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Article ID must not be empty.")
            .Must(BeAValidGuid)
            .WithMessage("Article ID must be a valid GUID.");
    }

    private bool BeAValidGuid(Guid guid)
    {
        return Guid.TryParse(guid.ToString(), out _);
    }
}
