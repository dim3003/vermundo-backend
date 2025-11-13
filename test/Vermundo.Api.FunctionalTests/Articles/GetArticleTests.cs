using Bogus;
using System.Net;
using System.Net.Http.Json;
using Vermundo.Api.Controllers.Articles;
using Vermundo.Api.FunctionalTests.Infrastructure;
using Vermundo.Application.Articles;

namespace Vermundo.Api.FunctionalTests.Articles;

public class GetArticleTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public GetArticleTests(FunctionalTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetArticle_ShouldReturnArticle_WhenArticleExists()
    {
        // Arrange
        var title = _faker.Lorem.Sentence();
        var body = _faker.Lorem.Paragraph(3);
        var imageUrl = _faker.Image.PicsumUrl();

        var createRequest = new CreateArticleRequest(title, body, imageUrl);
        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", createRequest);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var location = postResponse.Headers.Location?.ToString();
        Assert.NotNull(location);
        var articleId = location.Split('/').Last();

        // Act
        var getResponse = await HttpClient.GetAsync($"api/articles/{articleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var article = await getResponse.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.NotNull(article);
        Assert.Equal(title, article.Title);
        Assert.Equal(body, article.Body);
        Assert.Equal(imageUrl, article.ImageUrl);
        Assert.Equal(Guid.Parse(articleId), article.Id);
        Assert.True(article.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public async Task GetArticle_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync($"api/articles/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetArticle_ShouldReturnCorrectFields_WithNullImageUrl()
    {
        // Arrange
        var title = _faker.Lorem.Sentence();
        var body = _faker.Lorem.Paragraph();
        var createRequest = new CreateArticleRequest(title, body, null);

        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", createRequest);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var location = postResponse.Headers.Location?.ToString();
        var articleId = location?.Split('/').Last();
        Assert.NotNull(articleId);

        // Act
        var getResponse = await HttpClient.GetAsync($"api/articles/{articleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var article = await getResponse.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.NotNull(article);
        Assert.Equal(title, article.Title);
        Assert.Equal(body, article.Body);
        Assert.Null(article.ImageUrl);
        Assert.Equal(Guid.Parse(articleId), article.Id);
    }

    [Fact]
    public async Task GetArticle_ShouldMatchLocationHeaderId_WhenCreated()
    {
        // Arrange
        var request = new CreateArticleRequest(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(), null);
        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", request);

        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var location = postResponse.Headers.Location?.ToString();
        Assert.NotNull(location);
        var articleId = location.Split('/').Last();

        // Act
        var response = await HttpClient.GetAsync($"api/articles/{articleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var article = await response.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.Equal(Guid.Parse(articleId), article!.Id);
    }
}

