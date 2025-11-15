using Vermundo.Domain.Newsletters;

namespace Vermundo.Application.Email;

public interface INewsletterEmailContentFactory
{
    NewsletterEmailMessage CreateConfirmationEmail(
        NewsletterSubscriber subscriber,
        string confirmationToken
    );
}
