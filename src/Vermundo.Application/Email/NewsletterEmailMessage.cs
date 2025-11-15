namespace Vermundo.Application.Email;

public sealed record NewsletterEmailMessage(
    string To,
    string Subject,
    string HtmlBody,
    string? TextBody = null
);
