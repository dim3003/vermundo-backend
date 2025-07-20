using Vermundo.Application.IntegrationTests.Infrastructure;
using Vermundo.Application.Articles;
using Vermundo.TestUtils;

namespace Vermundo.Application.IntegrationTests.Articles;

public class CreateArticleTests : BaseIntegrationTest
{
    private readonly CreateArticleCommand _command;

    public CreateArticleTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        var createArticleCommandFactory = new CreateArticleCommandFactory();
        _command = createArticleCommandFactory.Create();
    }


    [Fact]
    public async Task CreateArticle_ShouldReturnResultSuccess_WhenCommandIsValid()
    {
        // Act
        var result = await Sender.Send(_command);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
