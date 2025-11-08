using Bogus;
using Vermundo.Application.Newsletter;

namespace Vermundo.Application.Unittests.Newsletter.SubscribeToNewsletter;

public class SubscribeToNewsletterCommandValidatorTests
{
    private readonly SubscribeToNewsletterCommandValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Validate_ValidEmail_ReturnsSuccess()
    {
        var command = new SubscribeToNewsletterCommand(_faker.Internet.Email());

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-an-email")]
    public async Task Validate_InvalidEmail_ReturnsFailure(string email)
    {
        var command = new SubscribeToNewsletterCommand(email);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }
}
