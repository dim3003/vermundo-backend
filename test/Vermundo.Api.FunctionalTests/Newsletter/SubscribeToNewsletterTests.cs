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
    }

    [Fact]
    public async Task Subscribe_ShouldReturnBadRequest_WhenEmailIsInvalid()
    {
        var request = new SubscribeToNewsletterRequest("not-an-email");

        var response = await HttpClient.PostAsJsonAsync("api/newsletter/subscribe", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
