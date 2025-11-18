using FluentValidation;

namespace Vermundo.Application.Newsletter;

internal sealed class ConfirmNewsletterSubscriptionCommandValidator 
    : AbstractValidator<ConfirmNewsletterSubscriptionCommand>
{
    public ConfirmNewsletterSubscriptionCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

