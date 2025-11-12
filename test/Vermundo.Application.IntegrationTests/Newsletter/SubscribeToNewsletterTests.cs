using Bogus;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Application.Newsletter;
using AppValidationException = Vermundo.Application.Exceptions.ValidationException;

namespace Vermundo.Application.IntegrationTests.Newsletters;

public class SubscribeToNewsletterTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public SubscribeToNewsletterTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SubscribeToNewsletter_ShouldReturnResultSuccess_WhenCommandIsValid()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var command = new SubscribeToNewsletterCommand(email);

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SubscribeToNewsletter_ShouldThrowValidationException_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new SubscribeToNewsletterCommand(string.Empty);

        // Act
        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(command));

        // Assert
        Assert.Contains(ex.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task SubscribeToNewsletter_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        // Arrange
        var invalidEmail = "not-an-email-address";
        var command = new SubscribeToNewsletterCommand(invalidEmail);

        // Act
        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(command));

        // Assert
        Assert.Contains(ex.Errors, e => e.PropertyName == "Email");
    }
}
