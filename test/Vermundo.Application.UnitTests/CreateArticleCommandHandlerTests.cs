using Moq;
namespace Vermundo.Application.UnitTests;

public class CreateArticleCommandHandlerTests
{
    [Fact]
    public async Task Handle_GivenValidCommand_ShouldSaveArticleToRepository()
    {
        // Arrange
        var mockRepo = new Mock<IArticleRepository>();
        var handler = new CreateArticleCommandHandler(mockRepo.Object);

        var command = new CreateArticleCommand
        {
            Title = "Test Title",
            Body = "First paragraph.\n\nSecond paragraph."
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockRepo.Verify(r =>
            r.AddAsync(It.Is<Article>(a =>
                a.Title == command.Title &&
                a.Body == command.Body)),
            Times.Once);
    }
}
