using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Vermundo.Api.Controllers.Newsletter;
using Vermundo.Api.FunctionalTests.Infrastructure;
using Vermundo.Domain.Newsletters;

namespace Vermundo.Api.FunctionalTests.Newsletter;

public class ConfirmNewsletterSubscriptionTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public ConfirmNewsletterSubscriptionTests(FunctionalTestsWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Confirm_ShouldReturnNoContent_AndCallNewsletterClient_WhenTokenIsValid()
    {
        // ARRANGE: create a pending subscriber through the real API
        var email = _faker.Internet.Email();

        var subscribeRequest = new SubscribeToNewsletterRequest(email);
        var subscribeResponse = await HttpClient.PostAsJsonAsync("api/newsletter/subscribe", subscribeRequest);

        Assert.Equal(HttpStatusCode.NoContent, subscribeResponse.StatusCode);

        // Grab the subscriber from the DB to get its token
        // Adjust DbSet/property names to your actual model:
        // e.g. DbContext.NewsletterSubscribers or DbContext.Subscribers, etc.
        var subscriber = await DbContext
            .Set<NewsletterSubscriber>() // or DbContext.NewsletterSubscribers
            .SingleAsync(s => s.Email == email.ToLower());

        var token = subscriber.ConfirmationToken; // adjust to your actual property name

        // ACT: confirm using the token
        var confirmRequest = new ConfirmNewsletterSubscriptionRequest(token);
        var confirmResponse = await HttpClient.PostAsJsonAsync("api/newsletter/confirm", confirmRequest);

        // ASSERT: HTTP + spy
        Assert.Equal(HttpStatusCode.NoContent, confirmResponse.StatusCode);

        Assert.Equal(1, _spy.ConfirmCallCount);
        Assert.NotNull(_spy.LastConfirmedProviderId);
        // because SpyNewsletterClient.SubscribeAsync returns 42,
        // and your app should propagate that into subscriber.InfomaniakId:
        Assert.Equal(42, _spy.LastConfirmedProviderId);
    }

    [Fact]
    public async Task Confirm_ShouldReturnNotFound_WhenSubscriberDoesNotExist()
    {
        var fakeToken = _faker.Random.Guid().ToString("N");

        var request = new ConfirmNewsletterSubscriptionRequest(fakeToken);
        var response = await HttpClient.PostAsJsonAsync("api/newsletter/confirm", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // No confirm call should have happened
        Assert.Equal(0, _spy.ConfirmCallCount);
        Assert.Null(_spy.LastConfirmedProviderId);
    }
}

