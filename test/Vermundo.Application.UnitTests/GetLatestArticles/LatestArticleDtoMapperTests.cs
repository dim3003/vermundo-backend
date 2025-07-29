using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.UnitTests.GetLatestArticles;

public class LatestArticleDtoMapperTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void ToLatestArticleDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var title = _faker.Lorem.Sentence();
        var body = string.Join(" ", _faker.Lorem.Words(5));
        var imageUrl = _faker.Image.PicsumUrl();
        var article = new Article(title, body, imageUrl)
        {
            CreatedAt = createdAt
        };

        // Act
        var dto = LatestArticleDtoMapper.ToLatestArticleDto(article);

        // Assert
        Assert.Equal(title, dto.Title);
        Assert.Equal(imageUrl, dto.ImageUrl);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(body, dto.BodyPreview);
    }

    [Fact]
    public void ToLatestArticleDto_BodyPreviewIsFirst50Words()
    {
        // Arrange
        var words = _faker.Lorem.Words(100).ToArray();
        var body = string.Join(" ", words);
        var article = new Article(_faker.Lorem.Sentence(), body, _faker.Image.PicsumUrl());

        // Act
        var dto = LatestArticleDtoMapper.ToLatestArticleDto(article);

        // Assert
        var previewWords = dto.BodyPreview.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(50, previewWords.Length);
        Assert.Equal(words[0], previewWords[0]);
        Assert.Equal(words[49], previewWords[49]);
    }
}