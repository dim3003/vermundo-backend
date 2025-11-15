using Microsoft.Extensions.Configuration;
using Vermundo.Application.Email;

public sealed class NewsletterEmailContentFactoryTests
{
    private readonly NewsletterEmailContentFactory _sut;

    public NewsletterEmailContentFactoryTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["App:BaseUrl"] = "https://vermundo.test"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _sut = new NewsletterEmailContentFactory(configuration);
    }

    [Fact]
    public void CreateConfirmationEmail_ShouldSetRecipientAndSubject()
    {
        // Arrange
        var email = "user@example.com";
        var token = "abc123";

        // Act
        var message = _sut.CreateConfirmationEmail(email, token);

        // Assert
        Assert.Equal(email, message.To);
        Assert.Equal("Confirmez votre inscription à la newsletter Vermundo", message.Subject);
    }

    [Fact]
    public void CreateConfirmationEmail_ShouldIncludeConfirmationLinkInHtmlAndText()
    {
        // Arrange
        var email = "user@example.com";
        var token = "abc123";

        // Act
        var message = _sut.CreateConfirmationEmail(email, token);

        // Assert
        var expectedLink = "https://vermundo.test/newsletter/confirm?token=abc123";

        // HTML body invariants
        Assert.Contains(expectedLink, message.HtmlBody);
        Assert.Contains("<a", message.HtmlBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Confirmer mon inscription", message.HtmlBody);

        // Text body invariants
        Assert.NotNull(message.TextBody);
        Assert.Contains(expectedLink, message.TextBody!);
        Assert.Contains("Merci de votre inscription", message.TextBody!);
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenBaseUrlMissing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>()) // no App:BaseUrl
            .Build();

        // Act
        var ex = Assert.Throws<InvalidOperationException>(
            () => new NewsletterEmailContentFactory(configuration));

        // Assert
        Assert.Equal("App:BaseUrl is not configured.", ex.Message);
    }
}
