using Bogus;
using System.Net;
using System.Net.Http.Json;
using Vermundo.Api.FunctionalTests.Infrastructure;
using Vermundo.Application.Articles;

namespace Vermundo.Api.FunctionalTests.Articles;

public class DeleteArticleTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public DeleteArticleTests(FunctionalTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task DeleteArticle_ShouldReturnNoContent_WhenArticleExists()
    {
        var createRequest = new CreateArticleCommand(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(3),
            _faker.Image.PicsumUrl()
        );

        var postResponse = await HttpClient.PostAsJsonAsync("api/articles", createRequest);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        Assert.NotNull(postResponse.Headers.Location);

        var location = postResponse.Headers.Location.ToString();
        var articleId = location.Split('/').Last();

        var deleteResponse = await HttpClient.DeleteAsync($"api/articles/{articleId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await HttpClient.GetAsync($"api/articles/{articleId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteArticle_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var response = await HttpClient.DeleteAsync($"api/articles/{nonExistentId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteArticle_ShouldReturnBadRequest_WhenGuidIsEmpty()
    {
        var response = await HttpClient.DeleteAsync("api/articles/00000000-0000-0000-0000-000000000000");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

