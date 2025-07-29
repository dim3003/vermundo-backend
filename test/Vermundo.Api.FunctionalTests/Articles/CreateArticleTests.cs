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

    [Fact]
    public async Task CreateArticle_ShouldReturnIsSuccess_WhenImageUrlIsValid()
    {
        var request = new CreateArticleRequest(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            "https://example.com/image.jpg"
        );

        var response = await HttpClient.PostAsJsonAsync("api/articles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.Location != null, "Location header is missing");
        var location = response.Headers.Location.ToString();
        Assert.Matches(@"\/articles\/[a-f0-9\-]+$", location);
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnBadRequest_WhenImageUrlIsInvalid()
    {
        var request = new CreateArticleRequest(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            "not-a-valid-url"
        );

        var response = await HttpClient.PostAsJsonAsync("api/articles", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnIsSuccess_WhenImageUrlIsNull()
    {
        var request = new CreateArticleRequest(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            null
        );

        var response = await HttpClient.PostAsJsonAsync("api/articles", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(response.Headers.Location != null, "Location header is missing");
        var location = response.Headers.Location.ToString();
        Assert.Matches(@"\/articles\/[a-f0-9\-]+$", location);
    }
    
    // Uncomment this once the GetArticle endpoint is done
    /*[Fact]
    public async Task CreateArticle_ShouldPersistImageUrl()
    {
        var expectedImageUrl = "https://cdn.example.com/my-image.png";
        var request = new CreateArticleRequest(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            expectedImageUrl
        );

        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        // Follow the Location header to get the article
        var getResponse = await HttpClient.GetAsync(postResponse.Headers.Location);
        getResponse.EnsureSuccessStatusCode();

        var article = await getResponse.Content.ReadFromJsonAsync<LatestArticleDto>();
        Assert.NotNull(article);
        Assert.Equal(expectedImageUrl, article.ImageUrl);
    }*/
}

