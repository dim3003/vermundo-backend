using Bogus;
using Vermundo.Application.Newsletter;

namespace Vermundo.Application.Unittests.Newsletter.ConfirmNewsletterSubscription;

public class ConfirmNewsletterSubscriptionCommandValidatorTests
{
    private readonly ConfirmNewsletterSubscriptionCommandValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Validate_ValidToken_ReturnsSuccess()
    {
        // Arrange
        // Use something that looks like a realistic token – but the validator only cares about non-empty.
        var command = new ConfirmNewsletterSubscriptionCommand(_faker.Random.String2(32));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidToken_ReturnsFailure(string token)
    {
        // Arrange
        var command = new ConfirmNewsletterSubscriptionCommand(token);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Token");
    }
}
