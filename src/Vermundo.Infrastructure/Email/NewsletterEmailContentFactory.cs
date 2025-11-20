using Microsoft.Extensions.Configuration;

namespace Vermundo.Application.Email;

public sealed class NewsletterEmailContentFactory : INewsletterEmailContentFactory
{
    private readonly string _appBaseUrl;

    public NewsletterEmailContentFactory(IConfiguration configuration)
    {
        _appBaseUrl =
            configuration["App:BaseUrl"]
            ?? throw new InvalidOperationException("App:BaseUrl is not configured.");
    }

    public NewsletterEmailMessage CreateConfirmationEmail(string email, string confirmationToken)
    {
        var confirmationLink = $"{_appBaseUrl}/confirm-newsletter/{confirmationToken}";

        var subject = "Confirmez votre inscription à la newsletter Vermundo";

        var textBody = $"""
            Bonjour,

            Merci de votre inscription à notre Newsletter!

            Veuillez confirmer votre inscription en cliquant sur le lien ci-dessous:
            {confirmationLink}

            Si cet email vous est parvenu par erreur, vous pouvez ignorer celui-ci.
            """;

        var htmlBody = $"""
            <p>Bonjour,</p>
            <p>Merci de votre inscription à notre Newsletter!</p>
            <p>
                Veuillez confirmer votre inscription en cliquant sur le lien ci-dessous:<br/>
                <a href="{confirmationLink}">Confirmer mon inscription</a>
            </p>
            <p>Si cet email vous est parvenu par erreur, vous pouvez ignorer celui-ci.</p>
            """;

        return new NewsletterEmailMessage(
            To: email,
            Subject: subject,
            HtmlBody: htmlBody,
            TextBody: textBody
        );
    }
}
