using FluentValidation;

internal sealed class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ImageUrl)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .Must(IsValidAbsoluteUrl)
            .WithMessage(url => $"Image URL '{url}' must be a valid absolute HTTP/HTTPS url.");
    }

    private static bool IsValidAbsoluteUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
