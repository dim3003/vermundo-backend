using Vermundo.Domain.Newsletters;

namespace Vermundo.Domain.UnitTests.Newsletter;

public class NewsletterSubscriberTests
{
    [Fact]
    public void CreateUnconfirmed_ShouldInitialize_WithUnconfirmedStatus_AndToken_AndTimestamps()
    {
        // Arrange
        var email = "user@example.com";
        var token = "test-token";
        var now = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        var subscriber = NewsletterSubscriber.CreateUnconfirmed(email, token, now);

        // Assert
        Assert.NotEqual(Guid.Empty, subscriber.Id);
        Assert.Equal(email.ToLowerInvariant(), subscriber.Email);
        Assert.Equal(SubscriberStatus.Unconfirmed, subscriber.Status);
        Assert.Equal(token, subscriber.ConfirmationToken);
        Assert.Equal(now, subscriber.CreatedAt);
        Assert.Null(subscriber.ConfirmedAt);
    }

    [Fact]
    public void Confirm_ShouldSetStatusToConfirmed_AndSetConfirmedAt_WhenTokenMatches()
    {
        // Arrange
        var email = "user@example.com";
        var token = "test-token";
        var createdAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var confirmedAt = createdAt.AddMinutes(5);
        var subscriber = NewsletterSubscriber.CreateUnconfirmed(email, token, createdAt);
        // Act
        var result = subscriber.Confirm(token, confirmedAt);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriberStatus.Confirmed, subscriber.Status);
        Assert.Equal(confirmedAt, subscriber.ConfirmedAt);
    }
    
    [Fact]
    public void Confirm_ShouldReturnFailure_WhenTokenDoesNotMatch()
    {
        // Arrange
        var email = "user@example.com";
        var token = "test-token";
        var subscriber = NewsletterSubscriber.CreateUnconfirmed(
            email,
            token,
            DateTimeOffset.UtcNow);
        // Act
        var result = subscriber.Confirm("wrong-token", DateTimeOffset.UtcNow);
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(NewsletterSubscriberErrors.InvalidConfirmationToken, result.Error);
        Assert.Equal(SubscriberStatus.Unconfirmed, subscriber.Status);
        Assert.Null(subscriber.ConfirmedAt);
    }
    
    [Fact]
    public void Confirm_ShouldBeIdempotent_WhenAlreadyConfirmed_WithSameToken()
    {
        // Arrange
        var email = "user@example.com";
        var token = "test-token";
        var createdAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var firstConfirmation = createdAt.AddMinutes(5);
        var secondConfirmation = createdAt.AddMinutes(10);
        var subscriber = NewsletterSubscriber.CreateUnconfirmed(email, token, createdAt);
        subscriber.Confirm(token, firstConfirmation);
        // Act
        var result = subscriber.Confirm(token, secondConfirmation);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriberStatus.Confirmed, subscriber.Status);
        // We keep the first confirmation timestamp
        Assert.Equal(firstConfirmation, subscriber.ConfirmedAt);
    }
    
    [Fact]
    public void Confirm_ShouldReturnFailure_WhenAlreadyConfirmed_WithDifferentToken()
    {
        // Arrange
        var email = "user@example.com";
        var token = "test-token";
        var createdAt = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var subscriber = NewsletterSubscriber.CreateUnconfirmed(email, token, createdAt);
        subscriber.Confirm(token, createdAt.AddMinutes(1));
        // Act
        var result = subscriber.Confirm("other-token", createdAt.AddMinutes(2));
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(NewsletterSubscriberErrors.InvalidConfirmationToken, result.Error);
        Assert.Equal(SubscriberStatus.Confirmed, subscriber.Status);
    }
}

