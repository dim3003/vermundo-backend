using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.TestUtils;

namespace Vermundo.Application.IntegrationTests.Articles;

public class GetLatestArticlesTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly CreateArticleCommandFactory _commandFactory = new();
    private readonly Faker _faker = new();

    public GetLatestArticlesTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturn_EmptyList_WhenNoArticlesExist()
    {
        var query = new GetLatestArticlesQuery();

        var result = await Sender.Send(query);

        Assert.NotNull(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetLatestArticles_ShouldReturn_LatestArticles_WithCorrectFields()
    {
        // Arrange: create 4 articles with different creation times
        var articles = new List<CreateArticleCommand>
        {
            _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(3), "https://img1.jpg"),
            _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(3), "https://img2.jpg"),
            _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(3), "https://img3.jpg"),
            _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(3), "https://img4.jpg"),
        };

        foreach (var command in articles)
        {
            await Sender.Send(command);
        }

        var query = new GetLatestArticlesQuery();

        // Act
        var result = await Sender.Send(query);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Value.Count <= 3);
        foreach (var dto in result.Value)
        {
            Assert.False(string.IsNullOrWhiteSpace(dto.Title));
            Assert.True(dto.CreatedAt > DateTime.MinValue);
            Assert.True(dto.BodyPreview.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= 50);
        }
        // Ensure articles are ordered by CreatedAt descending
        Assert.True(result.Value.SequenceEqual(result.Value.OrderByDescending(a => a.CreatedAt)));
    }

    [Fact]
    public async Task GetLatestArticles_BodyPreview_ShouldBeFirst50Words()
    {
        var body = string.Join(" ", Enumerable.Range(1, 100).Select(i => $"word{i}"));
        var command = _commandFactory.Create(_faker.Lorem.Sentence(), body, "https://img.jpg");
        await Sender.Send(command);

        var query = new GetLatestArticlesQuery();
        var result = await Sender.Send(query);

        var preview = result.Value.First().BodyPreview;
        var previewWords = preview.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Assert.True(previewWords.Length <= 50);
        Assert.Equal(string.Join(" ", body.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(50)), preview);
    }

}
