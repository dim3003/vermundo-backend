using Vermundo.Application.Abstractions;
using Vermundo.Application.Email;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Application.Newsletter;

public class NewsletterSubscriptionService : INewsletterSubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfirmationTokenGenerator _tokenGenerator;
    private readonly INewsletterEmailContentFactory _emailContentFactory;
    private readonly IEmailSender _emailSender;
    private readonly INewsletterClient _newsletterClient;

    public NewsletterSubscriptionService(
        IUnitOfWork unitOfWork,
        IConfirmationTokenGenerator tokenGenerator,
        INewsletterEmailContentFactory emailContentFactory,
        IEmailSender emailSender,
        INewsletterClient newsletterClient)
    {
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
        _emailContentFactory = emailContentFactory;
        _emailSender = emailSender;
        _newsletterClient = newsletterClient;
    }

    public async Task<Result> SubscribeAsync(string rawEmail, CancellationToken ct = default)
    {
        var email = rawEmail.Trim().ToLowerInvariant();
        var nowUtc = DateTimeOffset.UtcNow;

        var existing = await _unitOfWork.Subscriber.GetByEmailAsync(email, ct);

        if (existing is not null && existing.IsConfirmed)
        {
            return Result.Success();
        }

        var token = _tokenGenerator.Generate();

        NewsletterSubscriber subscriber;
        if (existing is null)
        {
            subscriber = NewsletterSubscriber.CreateUnconfirmed(email, token, nowUtc);
            await _unitOfWork.Subscriber.AddAsync(subscriber);
        }
        else
        {
            existing.SetConfirmationToken(token, nowUtc);
            subscriber = existing;
        }

        await _unitOfWork.SaveChangesAsync(ct);

        await _newsletterClient.SubscribeAsync(email, ct);

        var emailMessage = _emailContentFactory.CreateConfirmationEmail(email, token);
        await _emailSender.SendAsync(emailMessage, ct);

        return Result.Success();
    }
}

