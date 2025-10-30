using Bogus;
using System.Net;
using System.Net.Http.Json;
using Vermundo.Api.FunctionalTests.Infrastructure;
using Vermundo.Application.Articles;

namespace Vermundo.Api.FunctionalTests.Articles;

public class UpdateArticleTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public UpdateArticleTests(FunctionalTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateArticle_ShouldReturnOk_WhenArticleExists()
    {
        // Arrange: create an article
        var createRequest = new CreateArticleCommand(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(3),
            _faker.Image.PicsumUrl()
        );

        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", createRequest);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        var location = postResponse.Headers.Location!.ToString();
        var articleId = location.Split('/').Last();

        // Act: update the article's ImageUrl
        var newImageUrl = _faker.Image.PicsumUrl();
        var updateRequest = new UpdateArticleCommand(Guid.Parse(articleId), newImageUrl);
        var putResponse = await HttpClient.PutAsJsonAsync($"api/articles/{articleId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        // Verify the article was actually updated
        var getResponse = await HttpClient.GetAsync($"api/articles/{articleId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var article = await getResponse.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.Equal(newImageUrl, article!.ImageUrl);
    }

    [Fact]
    public async Task UpdateArticle_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new UpdateArticleCommand(nonExistentId, _faker.Image.PicsumUrl());

        var response = await HttpClient.PutAsJsonAsync($"api/articles/{nonExistentId}", updateRequest);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateArticle_ShouldReturnBadRequest_WhenGuidIsEmpty()
    {
        var updateRequest = new UpdateArticleCommand(Guid.Empty, _faker.Image.PicsumUrl());
        var response = await HttpClient.PutAsJsonAsync("api/articles/00000000-0000-0000-0000-000000000000", updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateArticle_ShouldReturnBadRequest_WhenImageUrlIsInvalid()
    {
        // Arrange: create an article
        var createRequest = new CreateArticleCommand(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(3),
            _faker.Image.PicsumUrl()
        );

        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", createRequest);
        var location = postResponse.Headers.Location!.ToString();
        var articleId = location.Split('/').Last();

        // Act: attempt to update with invalid URL
        var invalidUrl = "not-a-valid-url";
        var updateRequest = new UpdateArticleCommand(Guid.Parse(articleId), invalidUrl);
        var putResponse = await HttpClient.PutAsJsonAsync($"api/articles/{articleId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
    }
}
