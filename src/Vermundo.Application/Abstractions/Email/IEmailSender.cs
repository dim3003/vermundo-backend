namespace Vermundo.Application.Email;

public interface IEmailSender
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        string? textBody = null,
        CancellationToken ct = default);

    Task SendAsync(
        NewsletterEmailMessage message,
        CancellationToken ct = default);
}
