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
    public async Task SubscribeAsync_ShouldCallCorrectUrl_AndReturnSubscriberId_OnSuccess()
    {
        // Arrange
        const int expectedId = 46980;
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.OK,
            ResponseContent =
                """
                {
                  "result": "success",
                  "data": {
                    "id": 46980,
                    "email": "user@example.com",
                    "status": "junk",
                    "created_at": 1762714429
                  }
                }
                """
        };
        var client = CreateClient(handler);
        var email = "user@example.com";

        // Act
        var returnedId = await client.SubscribeAsync(email);

        // Assert – HTTP request
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal(
            "https://api.infomaniak.com/1/newsletters/test-domain/subscribers",
            handler.LastRequest.RequestUri!.ToString());

        // Assert – body payload
        Assert.NotNull(handler.LastRequestBody);
        using var doc = JsonDocument.Parse(handler.LastRequestBody!);
        Assert.Equal(email, doc.RootElement.GetProperty("email").GetString());

        // Assert – returned ID
        Assert.Equal(expectedId, returnedId);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldThrowInvalidOperationException_WhenSuccessResponseHasNoId()
    {
        // Arrange: success status but missing/invalid id
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.OK,
            ResponseContent =
                """
                {
                  "result": "success",
                  "data": {
                    "email": "user@example.com",
                    "status": "junk",
                    "created_at": 1762714429
                  }
                }
                """
        };

        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => client.SubscribeAsync("user@example.com"));

        // Assert
        Assert.Contains("Infomaniak response did not contain a valid subscriber id", ex.Message);
    }

    [Fact]
    public async Task SubscribeAsync_ShouldThrowHttpRequestException_OnBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.BadRequest,
            ResponseContent = "{\"error\":\"invalid email\"}"
        };
        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<HttpRequestException>(
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
        var ex = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.SubscribeAsync("user@example.com"));

        // Assert
        Assert.Contains("authentication failed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ConfirmAsync_ShouldCallCorrectUrl_AndSendActiveStatus_OnSuccess()
    {
        // Arrange
        const int providerId = 46980;

        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.OK,
            ResponseContent =
                """
                {
                  "result": "success",
                  "data": {
                    "id": 46980,
                    "email": "user@example.com",
                    "status": "active",
                    "created_at": 1763032749
                  }
                }
                """
        };

        var client = CreateClient(handler);

        // Act
        await client.ConfirmAsync(providerId);

        // Assert – HTTP request
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Put, handler.LastRequest!.Method);
        Assert.Equal(
            "https://api.infomaniak.com/1/newsletters/test-domain/subscribers/46980",
            handler.LastRequest.RequestUri!.ToString());

        // Assert – body payload
        Assert.NotNull(handler.LastRequestBody);
        using var doc = JsonDocument.Parse(handler.LastRequestBody!);

        // Expecting { "status": "active" } at minimum
        Assert.Equal("active", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task ConfirmAsync_ShouldThrowHttpRequestException_OnBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.BadRequest,
            ResponseContent = "{\"error\":\"invalid status\"}"
        };

        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ConfirmAsync(12345));

        // Assert
        Assert.Contains("Infomaniak rejected the request", ex.Message);
        Assert.Contains("invalid status", ex.Message);
    }

    [Fact]
    public async Task ConfirmAsync_ShouldThrowOnUnauthorized()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = HttpStatusCode.Unauthorized,
            ResponseContent = "unauthorized"
        };

        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ConfirmAsync(12345));

        // Assert
        Assert.Contains("authentication failed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ConfirmAsync_ShouldThrowOnTooManyRequests()
    {
        // Arrange
        var handler = new StubHttpMessageHandler
        {
            StatusCodeToReturn = (HttpStatusCode)429,
            ResponseContent = "rate limit"
        };

        var client = CreateClient(handler);

        // Act
        var ex = await Assert.ThrowsAsync<HttpRequestException>(
            () => client.ConfirmAsync(12345));

        // Assert
        Assert.Contains("rate limit", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
