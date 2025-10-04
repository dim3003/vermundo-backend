using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Domain.Articles;
using AppValidationException = Vermundo.Application.Exceptions.ValidationException;

namespace Vermundo.Application.IntegrationTests.Articles;

public class DeleteArticleTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public DeleteArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task DeleteArticle_ShouldSucceed_WhenArticleExists()
    {
        var createCommand = new CreateArticleCommand(
            Title: _faker.Lorem.Sentence(),
            Body: _faker.Lorem.Paragraph(3),
            ImageUrl: _faker.Image.PicsumUrl()
        );
        var createdArticleResult = await Sender.Send(createCommand);
        var createdArticleId = createdArticleResult.Value;

        var deleteCommand = new DeleteArticleCommand(createdArticleId);
        await Sender.Send(deleteCommand);

        var queryResult = await Sender.Send(new GetArticleQuery(createdArticleId));
        Assert.False(queryResult.IsSuccess);
        Assert.Equal(ArticleErrors.NotFound, queryResult.Error);
    }

    [Fact]
    public async Task DeleteArticle_ShouldThrow_WhenArticleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var deleteCommand = new DeleteArticleCommand(nonExistentId);

        var deleteResult = await Sender.Send(deleteCommand);

        Assert.False(deleteResult.IsSuccess);
        Assert.Equal(ArticleErrors.NotFound, deleteResult.Error);
    }

    [Fact]
    public async Task DeleteArticle_ShouldThrow_WhenGuidIsEmpty()
    {
        var deleteCommand = new DeleteArticleCommand(Guid.Empty);

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(deleteCommand));
        Assert.Contains(ex.Errors, e => e.PropertyName == "Id");
    }
}

