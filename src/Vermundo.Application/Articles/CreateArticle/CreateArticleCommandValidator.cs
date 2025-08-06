using FluentValidation;

namespace Vermundo.Application.Articles;

internal sealed class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
{
    private const int TitleMinLength = 3;
    private const int TitleMaxLength = 100;
    private const int BodyMinLength = 50;
    private const int BodyMaxLength = 10000;
    private const int ImageUrlMaxLength = 512;

    public CreateArticleCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().Length(TitleMinLength, TitleMaxLength);

        RuleFor(x => x.Body)
            .NotEmpty()
            .Length(BodyMinLength, BodyMaxLength)
            .Must(NotContainScriptTags)
            .WithMessage("Body must not contain script tags.");

        RuleFor(x => x.ImageUrl)
            .Custom(
                (imageUrl, context) =>
                {
                    if (string.IsNullOrWhiteSpace(imageUrl))
                        return;

                    if (imageUrl.Length > ImageUrlMaxLength)
                    {
                        context.AddFailure(
                            "ImageUrl",
                            $"Image URL must not exceed {ImageUrlMaxLength} characters."
                        );
                    }

                    if (!IsValidAbsoluteUrl(imageUrl))
                    {
                        context.AddFailure(
                            "ImageUrl",
                            $"Image URL '{imageUrl}' must be a valid URL."
                        );
                    }
                }
            );
    }

    private static bool NotContainScriptTags(string body) =>
        !body.Contains("<script", StringComparison.OrdinalIgnoreCase);

    private static bool IsValidAbsoluteUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
