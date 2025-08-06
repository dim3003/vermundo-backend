using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Domain.Articles;
using Vermundo.TestUtils;

namespace Vermundo.Application.IntegrationTests.Articles;

public class GetArticleTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly CreateArticleCommandFactory _commandFactory = new();
    private readonly Faker _faker = new();

    public GetArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetArticle_ShouldReturnSuccessAndCorrectArticle_WhenArticleExists()
    {
        // Arrange
        var title = _faker.Lorem.Sentence();
        var body = _faker.Lorem.Paragraph(3);
        var imageUrl = _faker.Image.PicsumUrl();

        var command = _commandFactory.Create(title, body, imageUrl);
        var createResult = await Sender.Send(command);

        var articleId = createResult.Value;
        var query = new GetArticleQuery(articleId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var article = result.Value;

        Assert.Equal(articleId, article.Id);
        Assert.Equal(title, article.Title);
        Assert.Equal(body, article.Body);
        Assert.Equal(imageUrl, article.ImageUrl);
    }


    [Fact]
    public async Task GetArticle_ShouldReturnFailure_WhenArticleDoesNotExist()
    {
        // Arrange
        var nonExistentArticleId = Guid.NewGuid();
        var query = new GetArticleQuery(nonExistentArticleId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(ArticleErrors.NotFound, result.Error);
    }
}
