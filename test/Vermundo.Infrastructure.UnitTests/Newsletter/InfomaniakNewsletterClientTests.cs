using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vermundo.TestUtils;
using Vermundo.Infrastructure.Newsletter;

namespace Vermundo.Infrastructure.UnitTests.Newsletter;

public class InfomaniakNewsletterClientTests
{
    private static InfomaniakNewsletterClient CreateClient(
        StubHttpMessageHandler handler,
        string? tokenOverride = null,
        string? domainOverride = null)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.infomaniak.com/1/")
        };

        var options = Options.Create(new InfomaniakNewsletterOptions
        {
            ApiToken = tokenOverride ?? "test-token",
            Domain = domainOverride ?? "test-domain"
        });

        var logger = new LoggerFactory().CreateLogger<InfomaniakNewsletterClient>();

        return new InfomaniakNewsletterClient(httpClient, options, logger);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldCallCorrectUrl_AndReturnOnSuccess()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.OK,
            ResponseContent = "{}"
        };
        var client = CreateClient(handler);
        var email = "user@example.com";

        // Act (no exception means success)
        await client.SubscribeAsync(email);

        // Assert
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal(
            "https://api.infomaniak.com/1/newsletters/test-domain/subscribers",
            handler.LastRequest.RequestUri!.ToString());

        // Body
        Assert.NotNull(handler.LastRequestBody);
        using var doc = JsonDocument.Parse(handler.LastRequestBody!);
        Assert.Equal(email, doc.RootElement.GetProperty("email").GetString());
    }

    [Fact]
    public async Task SubscribeAsync_ShouldThrowInvalidOperationException_OnBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.BadRequest,
            ResponseContent = "{\"error\":\"invalid email\"}"
        };
        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SubscribeAsync("invalid-email"));

        // Assert
        Assert.Contains("Infomaniak rejected the request", ex.Message);
        Assert.Contains("invalid email", ex.Message);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldThrowOnUnauthorized()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.Unauthorized,
            ResponseContent = "unauthorized"
        };
        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SubscribeAsync("user@example.com"));

        // Assert
        Assert.Contains("authentication failed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
