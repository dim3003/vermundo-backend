using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
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
        // Build the email message using MimeKit
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = textBody ?? string.Empty
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        // Choose TLS mode based on your config
        SecureSocketOptions socketOptions =
            _options.Port switch
            {
                465 => SecureSocketOptions.SslOnConnect,       // implicit TLS
                587 => SecureSocketOptions.StartTls,           // STARTTLS
                _ => _options.UseSsl ? SecureSocketOptions.SslOnConnect
                                     : SecureSocketOptions.StartTls
            };

        // Connect
        await client.ConnectAsync(_options.Host, _options.Port, socketOptions, ct);

        // Authenticate
        await client.AuthenticateAsync(_options.UserName, _options.Password, ct);

        // Send the email
        await client.SendAsync(message, ct);

        // Disconnect
        await client.DisconnectAsync(true, ct);
    }

    public async Task SendAsync(
        NewsletterEmailMessage emailMessage,
        CancellationToken ct = default)
    {
        await SendAsync(
            emailMessage.To,
            emailMessage.Subject,
            emailMessage.HtmlBody,
            emailMessage.TextBody,
            ct);
    }
}
