namespace Vermundo.Application.Email;

public interface INewsletterEmailContentFactory
{
    NewsletterEmailMessage CreateConfirmationEmail(
        string email,
        string confirmationToken
    );
}
