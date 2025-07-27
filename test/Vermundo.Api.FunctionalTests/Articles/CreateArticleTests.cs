using Bogus;
using System.Net;
using System.Net.Http.Json;
using Vermundo.Api.Controllers.Articles;
using Vermundo.Api.FunctionalTests.Infrastructure;

namespace Vermundo.Api.FunctionalTests.Articles;

public class CreateArticleTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public CreateArticleTests(FunctionalTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnIsSuccess_WhenValid()
    {
        var request = new CreateArticleRequest(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph());

        var response = await HttpClient.PostAsJsonAsync("api/articles",
            request
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.Location != null, "Location header is missing");

        var location = response.Headers.Location.ToString();
        Assert.Matches(@"\/articles\/[a-f0-9\-]+$", location);
    }
}
