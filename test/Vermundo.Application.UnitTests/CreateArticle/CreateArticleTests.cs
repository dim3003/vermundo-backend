using Bogus;
using Moq;
using Vermundo.Application.Articles;
using Vermundo.Domain.Articles;
namespace Vermundo.Application.UnitTests.CreateArticle;

public class CreateArticleTests 
{
    private readonly CreateArticleCommand _command;

    public CreateArticleTests()
    {
        var factory = new CreateArticleCommandFactory();
        _command = factory.Create();
    }

    [Fact]
    public async Task Handle_GivenValidCommand_ShouldSaveArticleToRepository()
    {
        // Arrange
        var mockRepo = new Mock<IArticleRepository>();
        var handler = new CreateArticleCommandHandler(mockRepo.Object);

        // Act
        await handler.Handle(_command, CancellationToken.None);

        // Assert
        mockRepo.Verify(r =>
            r.AddAsync(It.Is<Article>(a =>
                a.Title == _command.Title &&
                a.Body == _command.Body)),
            Times.Once);
    }
}
