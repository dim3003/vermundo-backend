using Vermundo.Application.Email;

namespace Vermundo.TestUtils;

public sealed class FakeEmailSender : IEmailSender
{
    public List<NewsletterEmailMessage> SentMessages { get; } = new();

    public Task SendAsync(NewsletterEmailMessage emailMessage, CancellationToken ct = default)
    {
        SentMessages.Add(emailMessage);
        return Task.CompletedTask;
    }

    public Task SendAsync(string to, string subject, string htmlBody, string? textBody, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}

