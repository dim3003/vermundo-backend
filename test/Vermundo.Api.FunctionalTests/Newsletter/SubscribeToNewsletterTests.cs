using System.Net;
using System.Net.Http.Json;
using Bogus;
using Vermundo.Api.Controllers.Newsletter;
using Vermundo.Api.FunctionalTests.Infrastructure;

namespace Vermundo.Api.FunctionalTests.Newsletter;

public class SubscribeNewsletterTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public SubscribeNewsletterTests(FunctionalTestsWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Subscribe_ShouldReturnNoContent_WhenEmailIsValid()
    {
        var email = _faker.Internet.Email();

        var request = new SubscribeToNewsletterRequest(email);
        var response = await HttpClient.PostAsJsonAsync("api/newsletter/subscribe", request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Single(_spy.Calls);
        Assert.Equal(email, _spy.Calls[0].Email);
    }

    [Fact]
    public async Task Subscribe_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        var request = new SubscribeToNewsletterRequest("not-an-email");

        var response = await HttpClient.PostAsJsonAsync("api/newsletter/subscribe", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(_spy.Calls);
    }

    [Fact]
    public async Task Subscribe_ShouldPropagateDomainErrors_AsProblemDetails()
    {
        _spy.FailWith = new HttpRequestException(
            "Infomaniak rate limit hit.",
            inner: null,
            statusCode: HttpStatusCode.TooManyRequests
        );

        var request = new SubscribeToNewsletterRequest(_faker.Internet.Email());

        var response = await HttpClient.PostAsJsonAsync("api/newsletter/subscribe", request);

        Assert.Equal((HttpStatusCode)429, response.StatusCode);
    }
}
