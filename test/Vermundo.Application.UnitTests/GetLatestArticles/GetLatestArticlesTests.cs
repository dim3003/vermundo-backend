using Moq;
using Vermundo.Application.Articles;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.UnitTests.GetLatestArticles;

public class GetLatestArticlesTests
{
    [Fact]
    public async Task Handle_Returns3LatestArticles_WithCorrectFields()
    {
        // Arrange
        var articles = new List<Article>
        {
            new Article("Title 1", "Body " + string.Join(" ", Enumerable.Repeat("word", 100)), "img1.jpg") { CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Article("Title 2", "Body " + string.Join(" ", Enumerable.Repeat("word", 100)), "img2.jpg") { CreatedAt = DateTime.UtcNow.AddDays(-2) },
            new Article("Title 3", "Body " + string.Join(" ", Enumerable.Repeat("word", 100)), "img3.jpg") { CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new Article("Title 4", "Body " + string.Join(" ", Enumerable.Repeat("word", 100)), "img4.jpg") { CreatedAt = DateTime.UtcNow.AddDays(-4) }
        };

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock
            .Setup(r => r.GetLatestAsync(3))
            .ReturnsAsync(articles.OrderByDescending(a => a.CreatedAt).Take(3).ToList());

        var handler = new GetLatestArticlesQueryHandler(articleRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLatestArticlesQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        foreach (var dto in result)
        {
            Assert.False(string.IsNullOrWhiteSpace(dto.Title));
            Assert.False(string.IsNullOrWhiteSpace(dto.ImageUrl));
            Assert.True(dto.CreatedAt > DateTime.MinValue);
            Assert.True(dto.BodyPreview.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= 50);
        }
    }

    [Fact]
    public async Task Handle_BodyPreview_IsFirst50Words()
    {
        // Arrange
        var body = string.Join(" ", Enumerable.Range(1, 100).Select(i => $"word{i}"));
        var article = new Article("Title", body, "img.jpg") { CreatedAt = DateTime.UtcNow };
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock
            .Setup(r => r.GetLatestAsync(3))
            .ReturnsAsync(new List<Article> { article });

        var handler = new GetLatestArticlesQueryHandler(articleRepositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetLatestArticlesQuery(), CancellationToken.None);

        // Assert
        var preview = result.Single().BodyPreview;
        var previewWords = preview.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(50, previewWords.Length);
        Assert.Equal("word1", previewWords[0]);
        Assert.Equal("word50", previewWords[49]);
    }
}
