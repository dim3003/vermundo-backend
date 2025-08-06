using Bogus;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.TestUtils;
using AppValidationException = Vermundo.Application.Exceptions.ValidationException;

namespace Vermundo.Application.IntegrationTests.Articles;

public class CreateArticleTests : BaseIntegrationTest
{
    private readonly CreateArticleCommandFactory _commandFactory = new();
    private readonly Faker _faker = new();

    public CreateArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }


    [Fact]
    public async Task CreateArticle_ShouldReturnResultSuccess_WhenCommandIsValid()
    {
        var command = _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(3), _faker.Image.PicsumUrl());

        var result = await Sender.Send(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnResultValidationFailure_WhenCommandIsInvalid()
    {
        var command = _commandFactory.Create(_faker.Lorem.Sentence(1), _faker.Random.String2(10001));

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(command));

        Assert.Contains(ex.Errors, e => e.PropertyName == "Body");
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnResultSuccess_WhenImageUrlIsValid()
    {
        var validImageUrl = "https://example.com/image.jpg";
        var command = _commandFactory.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(), validImageUrl);

        var result = await Sender.Send(command);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnResultValidationFailure_WhenImageUrlIsInvalid()
    {
        var invalidImageUrl = "not-a-valid-url";
        var command = _commandFactory.Create(_faker.Lorem.Sentence(3), _faker.Lorem.Paragraph(), invalidImageUrl);

        var ex = await Assert.ThrowsAsync<AppValidationException>(() => Sender.Send(command));

        Assert.Contains(ex.Errors, e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public async Task CreateArticle_ShouldReturnResultSuccess_WhenImageUrlIsNull()
    {
        var command = _commandFactory.Create(_faker.Lorem.Sentence(3), _faker.Lorem.Paragraph(), null);

        var result = await Sender.Send(command);

        Assert.True(result.IsSuccess);
    }
}
