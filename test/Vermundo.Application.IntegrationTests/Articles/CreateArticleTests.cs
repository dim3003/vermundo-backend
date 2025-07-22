using AppValidationException = Vermundo.Application.Exceptions.ValidationException;
using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.TestUtils;
using Bogus;

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
        var command = _commandFactory.Create();

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
}
