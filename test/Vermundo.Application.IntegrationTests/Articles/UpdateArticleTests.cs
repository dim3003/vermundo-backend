using Bogus;
using Vermundo.Application.Articles;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Domain.Articles;
using AppValidationException = Vermundo.Application.Exceptions.ValidationException;

namespace Vermundo.Application.IntegrationTests.Articles;

public class UpdateArticleTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UpdateArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateArticle_ShouldSucceed_WhenArticleExists()
    {
        var createCommand = new CreateArticleCommand(
            Title: _faker.Lorem.Sentence(),
            Body: _faker.Lorem.Paragraph(3),
            ImageUrl: _faker.Image.PicsumUrl()
        );
        var createdArticleResult = await Sender.Send(createCommand);
        var createdArticleId = createdArticleResult.Value;

        var updateCommand = new UpdateArticleCommand(createdArticleId, _faker.Image.PicsumUrl());
        var queryResult = await Sender.Send(updateCommand);
        Assert.True(queryResult.IsSuccess);

        var updatedArticle = await Sender.Send(new GetArticleQuery(createdArticleId));
        Assert.Equal(updateCommand.ImageUrl, updatedArticle.Value.ImageUrl);
    }

    [Fact]
    public async Task UpdateArticle_ShouldThrow_WhenArticleDoesNotExist()
    {
        var nonExistentId = Guid.NewGuid();
        var updateCommand = new UpdateArticleCommand(nonExistentId, _faker.Image.PicsumUrl());

        var updateResult = await Sender.Send(updateCommand);

        Assert.False(updateResult.IsSuccess);
        Assert.Equal(ArticleErrors.NotFound, updateResult.Error);
    }

    [Fact]
    public async Task UpdateArticle_ShouldThrow_WhenGuidIsEmpty()
    {
        var updateCommand = new UpdateArticleCommand(Guid.Empty, _faker.Image.PicsumUrl());

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(updateCommand));
        Assert.Contains(ex.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateArticle_ShouldThrow_WhenImageUrlIsEmpty()
    {
        var updateCommand = new UpdateArticleCommand(Guid.NewGuid(), string.Empty);

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(updateCommand));
        Assert.Contains(ex.Errors, e => e.PropertyName == "ImageUrl");
    }
}

