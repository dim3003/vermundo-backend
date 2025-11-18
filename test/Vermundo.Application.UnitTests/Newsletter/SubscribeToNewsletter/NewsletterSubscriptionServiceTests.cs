using Bogus;
using Moq;
using Vermundo.Application.Abstractions;
using Vermundo.Application.Email;
using Vermundo.Application.Newsletter;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Application.UnitTests.Newsletter.SubscribeToNewsletter;

public class NewsletterSubscriptionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new(MockBehavior.Strict);
    private readonly Mock<INewsletterSubscriberRepository> _subscriberRepositoryMock = new(MockBehavior.Strict);
    private readonly Mock<IConfirmationTokenGenerator> _tokenGeneratorMock = new(MockBehavior.Strict);
    private readonly Mock<INewsletterEmailContentFactory> _emailContentFactoryMock = new(MockBehavior.Strict);
    private readonly Mock<IEmailSender> _emailSenderMock = new(MockBehavior.Strict);
    private readonly Mock<INewsletterClient> _newsletterClientMock = new(MockBehavior.Strict);
    private readonly NewsletterSubscriptionService _service;
    private readonly Faker _faker = new();

    public NewsletterSubscriptionServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.Subscriber).Returns(_subscriberRepositoryMock.Object);

        _service = new NewsletterSubscriptionService(
            _unitOfWorkMock.Object,
            _tokenGeneratorMock.Object,
            _emailContentFactoryMock.Object,
            _emailSenderMock.Object,
            _newsletterClientMock.Object);
    }

    [Fact]
    public async Task SubscribeAsync_WhenSubscriberAlreadyConfirmed_ReturnsSuccessWithoutSideEffects()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var existing = NewsletterSubscriber.CreateUnconfirmed(email, "token", DateTimeOffset.UtcNow);
        existing.Confirm("token", DateTimeOffset.UtcNow.AddMinutes(1));

        _subscriberRepositoryMock
            .Setup(r => r.GetByEmailAsync(email.Trim().ToLowerInvariant(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _service.SubscribeAsync(email, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        _tokenGeneratorMock.Verify(g => g.Generate(), Times.Never);
        _subscriberRepositoryMock.Verify(r => r.AddAsync(It.IsAny<NewsletterSubscriber>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _newsletterClientMock.Verify(c => c.SubscribeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailContentFactoryMock.Verify(f => f.CreateConfirmationEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _emailSenderMock.Verify(s => s.SendAsync(It.IsAny<NewsletterEmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SubscribeAsync_WhenSubscriberDoesNotExist_CreatesSubscriberAndSendsEmail()
    {
        // Arrange
        var rawEmail = "   " + _faker.Internet.Email().ToUpperInvariant() + "   ";
        var normalizedEmail = rawEmail.Trim().ToLowerInvariant();
        var generatedToken = _faker.Random.AlphaNumeric(12);

        _subscriberRepositoryMock
            .Setup(r => r.GetByEmailAsync(normalizedEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync((NewsletterSubscriber?)null);

        NewsletterSubscriber? capturedSubscriber = null;
        _subscriberRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<NewsletterSubscriber>()))
            .Callback<NewsletterSubscriber>(s => capturedSubscriber = s)
            .Returns(Task.CompletedTask);

        _tokenGeneratorMock
            .Setup(g => g.Generate())
            .Returns(generatedToken);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _newsletterClientMock
            .Setup(c => c.SubscribeAsync(normalizedEmail, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var emailMessage = new NewsletterEmailMessage(normalizedEmail, "subject", "<p>body</p>", "body");
        _emailContentFactoryMock
            .Setup(f => f.CreateConfirmationEmail(normalizedEmail, generatedToken))
            .Returns(emailMessage);

        _emailSenderMock
            .Setup(s => s.SendAsync(emailMessage, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SubscribeAsync(rawEmail, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedSubscriber);
        Assert.Equal(normalizedEmail, capturedSubscriber!.Email);
        Assert.Equal(generatedToken, capturedSubscriber.ConfirmationToken);
        Assert.False(capturedSubscriber.IsConfirmed);

        _subscriberRepositoryMock.Verify(r => r.AddAsync(It.IsAny<NewsletterSubscriber>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _newsletterClientMock.Verify(c => c.SubscribeAsync(normalizedEmail, It.IsAny<CancellationToken>()), Times.Once);
        _emailContentFactoryMock.Verify(f => f.CreateConfirmationEmail(normalizedEmail, generatedToken), Times.Once);
        _emailSenderMock.Verify(s => s.SendAsync(emailMessage, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubscribeAsync_WhenSubscriberExistsButUnconfirmed_RegeneratesTokenAndSendsEmail()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = NewsletterSubscriber.CreateUnconfirmed(email, "old-token", DateTimeOffset.UtcNow.AddDays(-1));
        var newToken = _faker.Random.AlphaNumeric(16);

        _subscriberRepositoryMock
            .Setup(r => r.GetByEmailAsync(normalizedEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        _tokenGeneratorMock
            .Setup(g => g.Generate())
            .Returns(newToken);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _newsletterClientMock
            .Setup(c => c.SubscribeAsync(normalizedEmail, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var emailMessage = new NewsletterEmailMessage(normalizedEmail, "subject", "<p>body</p>", "body");
        _emailContentFactoryMock
            .Setup(f => f.CreateConfirmationEmail(normalizedEmail, newToken))
            .Returns(emailMessage);

        _emailSenderMock
            .Setup(s => s.SendAsync(emailMessage, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.SubscribeAsync(email, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newToken, existing.ConfirmationToken);
        Assert.False(existing.IsConfirmed);

        _subscriberRepositoryMock.Verify(r => r.AddAsync(It.IsAny<NewsletterSubscriber>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _newsletterClientMock.Verify(c => c.SubscribeAsync(normalizedEmail, It.IsAny<CancellationToken>()), Times.Once);
        _emailContentFactoryMock.Verify(f => f.CreateConfirmationEmail(normalizedEmail, newToken), Times.Once);
        _emailSenderMock.Verify(s => s.SendAsync(emailMessage, It.IsAny<CancellationToken>()), Times.Once);
    }
}

