using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Domain.Articles;

namespace Vermundo.Application.UnitTests.Articles.GetArticle;

public class ArticleDtoMapperTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void ToArticleDto_MapsAllFieldsCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = _faker.Lorem.Sentence(1);
        var body = _faker.Lorem.Paragraph(3);
        var imageUrl = _faker.Image.PicsumUrl();
        var createdAt = DateTime.UtcNow;

        var article = new Article(title, body, imageUrl)
        {
            Id = id,
            CreatedAt = createdAt
        };

        // Act
        var dto = ArticleDtoMapper.ToArticleDto(article);

        // Assert
        Assert.Equal(id, dto.Id);
        Assert.Equal(title, dto.Title);
        Assert.Equal(body, dto.Body);
        Assert.Equal(imageUrl, dto.ImageUrl);
        Assert.Equal(createdAt, dto.CreatedAt);
    }
}

