using FluentValidation;

namespace Vermundo.Application.Newsletter;

internal sealed class SubscribeToNewsletterCommandValidator : AbstractValidator<SubscribeToNewsletterCommand>{
    public SubscribeToNewsletterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
