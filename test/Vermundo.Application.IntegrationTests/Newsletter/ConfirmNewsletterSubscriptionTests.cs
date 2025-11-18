using Bogus;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Application.Newsletter;
using Vermundo.Domain.Newsletters;
using AppValidationException = Vermundo.Application.Exceptions.ValidationException;

namespace Vermundo.Application.IntegrationTests.Newsletters;

public class ConfirmNewsletterSubscriptionTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public ConfirmNewsletterSubscriptionTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ConfirmNewsletterSubscription_ShouldReturnSuccess_WhenTokenIsValid()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var subscribeCommand = new SubscribeToNewsletterCommand(email);
        var subscribeResult = await Sender.Send(subscribeCommand);
        
        // Get the token from the database
        var subscriber = DbContext.NewsletterSubscribers
            .FirstOrDefault(s => s.Email == email.ToLowerInvariant());
        Assert.NotNull(subscriber);
        
        var confirmCommand = new ConfirmNewsletterSubscriptionCommand(subscriber.ConfirmationToken);

        // Act
        var result = await Sender.Send(confirmCommand);

        // Assert
        Assert.True(result.IsSuccess);
        
        // Verify subscriber is confirmed in database
        var confirmedSubscriber = DbContext.NewsletterSubscribers
            .FirstOrDefault(s => s.Email == email.ToLowerInvariant());
        Assert.NotNull(confirmedSubscriber);
        Assert.Equal(SubscriberStatus.Confirmed, confirmedSubscriber.Status);
        Assert.NotNull(confirmedSubscriber.ConfirmedAt);
    }

    [Fact]
    public async Task ConfirmNewsletterSubscription_ShouldReturnFailure_WhenTokenDoesNotExist()
    {
        // Arrange
        var invalidToken = "non-existent-token";
        var command = new ConfirmNewsletterSubscriptionCommand(invalidToken);

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(NewsletterSubscriberErrors.NewsletterSubscriberNotFound, result.Error);
    }

    [Fact]
    public async Task ConfirmNewsletterSubscription_ShouldReturnFailure_WhenTokenIsInvalid()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var subscribeCommand = new SubscribeToNewsletterCommand(email);
        await Sender.Send(subscribeCommand);
        
        var wrongToken = "wrong-token-value";
        var command = new ConfirmNewsletterSubscriptionCommand(wrongToken);

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(NewsletterSubscriberErrors.NewsletterSubscriberNotFound, result.Error);
    }

    [Fact]
    public async Task ConfirmNewsletterSubscription_ShouldBeIdempotent_WhenConfirmingTwice()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var subscribeCommand = new SubscribeToNewsletterCommand(email);
        await Sender.Send(subscribeCommand);
        
        var subscriber = DbContext.NewsletterSubscribers
            .FirstOrDefault(s => s.Email == email.ToLowerInvariant());
        Assert.NotNull(subscriber);
        
        var confirmCommand = new ConfirmNewsletterSubscriptionCommand(subscriber.ConfirmationToken);

        // Act
        var firstResult = await Sender.Send(confirmCommand);
        var secondResult = await Sender.Send(confirmCommand);

        // Assert
        Assert.True(firstResult.IsSuccess);
        Assert.True(secondResult.IsSuccess);
        
        var confirmedSubscriber = DbContext.NewsletterSubscribers
            .FirstOrDefault(s => s.Email == email.ToLowerInvariant());
        Assert.NotNull(confirmedSubscriber);
        Assert.Equal(SubscriberStatus.Confirmed, confirmedSubscriber.Status);
    }

    [Fact]
    public async Task ConfirmNewsletterSubscription_ShouldThrowValidationException_WhenTokenIsEmpty()
    {
        // Arrange
        var command = new ConfirmNewsletterSubscriptionCommand(string.Empty);

        // Act
        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(command));

        // Assert
        Assert.Contains(ex.Errors, e => e.PropertyName == "Token");
    }
}
