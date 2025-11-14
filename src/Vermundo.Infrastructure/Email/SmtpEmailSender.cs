using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Vermundo.Application.Email;

namespace Vermundo.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        string? textBody = null,
        CancellationToken ct = default)
    {
        using var message = new MailMessage();

        message.From = new MailAddress(_options.FromAddress, _options.FromName);
        message.To.Add(new MailAddress(to));
        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        if (!string.IsNullOrEmpty(textBody))
        {
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(textBody, null, "text/plain"));
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html"));
        }

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            Credentials = new NetworkCredential(_options.UserName, _options.Password),
            EnableSsl = _options.UseSsl
        };

        if (_options.UseStartTls)
        {
            // With System.Net.Mail there's no explicit StartTLS toggle,
            // it's negotiated automatically on Port 587 when EnableSsl = true.
            client.EnableSsl = true;
        }

        // System.Net.Mail has no true async; this avoids blocking your API thread pool:
        await Task.Run(() => client.Send(message), ct);
    }
}
