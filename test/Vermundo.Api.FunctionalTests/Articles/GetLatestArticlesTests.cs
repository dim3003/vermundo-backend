using Bogus;
using System.Net;
using System.Net.Http.Json;
using Vermundo.Api.Controllers.Articles;
using Vermundo.Api.FunctionalTests.Infrastructure;
using Vermundo.Application.Articles;

namespace Vermundo.Api.FunctionalTests.Articles;

public class GetLatestArticlesTests : BaseFunctionalTests
{
    private readonly Faker _faker = new();

    public GetLatestArticlesTests(FunctionalTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturnEmptyList_WhenNoArticlesExist()
    {
        var response = await HttpClient.GetAsync("api/articles/latest");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var articles = await response.Content.ReadFromJsonAsync<List<LatestArticleDto>>();
        Assert.NotNull(articles);
        Assert.Empty(articles);
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturnLatestThreeArticles_WhenMultipleArticlesExist()
    {
        // Arrange - Create 4 articles
        var createdArticles = new List<(string Id, DateTime CreatedAt)>();
        for (int i = 1; i <= 4; i++)
        {
            var request = new CreateArticleRequest(
                _faker.Lorem.Sentence(),
                _faker.Lorem.Paragraph(),
                $"https://example.com/image{i}.jpg"
            );

            var response = await HttpClient.PostAsJsonAsync("api/articles", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var location = response.Headers.Location?.ToString();
            Assert.NotNull(location);
            createdArticles.Add((location.Split('/').Last(), DateTime.UtcNow));

            // Add small delay to ensure different creation times
            await Task.Delay(100);
        }

        // Act
        var getResponse = await HttpClient.GetAsync("api/articles/latest");
        var responseBody = await getResponse.Content.ReadAsStringAsync();
        Console.WriteLine("RAW RESPONSE:");
        Console.WriteLine(responseBody);


        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var articles = await getResponse.Content.ReadFromJsonAsync<List<LatestArticleDto>>();
        Assert.NotNull(articles);
        Assert.Equal(3, articles.Count);

        // Verify ordering (newest first)
        var orderedArticles = articles.OrderByDescending(a => a.CreatedAt).ToList();
        Assert.Equal(articles, orderedArticles);
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturnBodyPreviewWithMax50Words()
    {
        // Arrange - Create article with long body
        var body = string.Join(" ", Enumerable.Range(1, 100).Select(i => $"word{i}"));
        var request = new CreateArticleRequest(
            _faker.Lorem.Sentence(),
            body,
            "https://example.com/image.jpg"
        );

        await HttpClient.PostAsJsonAsync("api/articles", request);

        // Act
        var response = await HttpClient.GetAsync("api/articles/latest");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var articles = await response.Content.ReadFromJsonAsync<List<LatestArticleDto>>();
        Assert.NotNull(articles);
        Assert.NotEmpty(articles);

        var article = articles.First();
        var previewWords = article.BodyPreview.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.True(previewWords.Length <= 50);

        // Verify first 50 words are present in order
        var expectedPreview = string.Join(" ", Enumerable.Range(1, 50).Select(i => $"word{i}"));
        Assert.Equal(expectedPreview, article.BodyPreview);
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturnCorrectFields()
    {
        // Arrange
        var title = _faker.Lorem.Sentence();
        var body = _faker.Lorem.Paragraph();
        var imageUrl = "https://example.com/image.jpg";

        var request = new CreateArticleRequest(title, body, imageUrl);
        await HttpClient.PostAsJsonAsync("api/articles", request);

        // Act
        var response = await HttpClient.GetAsync("api/articles/latest");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var articles = await response.Content.ReadFromJsonAsync<List<LatestArticleDto>>();
        Assert.NotNull(articles);
        Assert.NotEmpty(articles);

        var article = articles.First();
        Assert.Equal(title, article.Title);
        Assert.Equal(imageUrl, article.ImageUrl);
        Assert.True(article.CreatedAt > DateTime.MinValue);
        Assert.False(string.IsNullOrWhiteSpace(article.BodyPreview));
    }
}
