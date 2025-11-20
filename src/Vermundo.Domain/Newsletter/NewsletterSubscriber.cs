using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Newsletters;

public class NewsletterSubscriber : Entity
{
    private NewsletterSubscriber() { }

    public string Email { get; private set; } = default!;
    public SubscriberStatus Status { get; private set; }
    public string ConfirmationToken { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public bool IsConfirmed => Status == SubscriberStatus.Confirmed;

    public static NewsletterSubscriber CreateUnconfirmed(
        string email,
        string confirmationToken,
        DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (string.IsNullOrWhiteSpace(confirmationToken))
            throw new ArgumentException("Confirmation token cannot be empty.", nameof(confirmationToken));

        return new NewsletterSubscriber
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            Status = SubscriberStatus.Unconfirmed,
            ConfirmationToken = confirmationToken,
            CreatedAt = nowUtc,
            ConfirmedAt = null
        };
    }

    public Result Confirm(string token, DateTimeOffset nowUtc)
    {
        if (token != ConfirmationToken)
        {
            return Result.Failure(NewsletterSubscriberErrors.InvalidConfirmationToken);
        }
        
        if (Status == SubscriberStatus.Confirmed)
        {
            return Result.Success();
        }
        
        Status = SubscriberStatus.Confirmed;
        ConfirmedAt = nowUtc;
        
        return Result.Success();
    }

    public void SetConfirmationToken(string token, DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be empty.", nameof(token));
        }

        ConfirmationToken = token;
        Status = SubscriberStatus.Unconfirmed;
        ConfirmedAt = null;
    }
}
