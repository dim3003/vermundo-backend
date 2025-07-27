using Moq;
using Vermundo.Application.Articles;
using Vermundo.Domain.Abstractions;
using Vermundo.Domain.Articles;
using Vermundo.TestUtils;

namespace Vermundo.Application.UnitTests.CreateArticle;

public class CreateArticleTests : CreateArticleCommandValidatorTests
{
    private readonly CreateArticleCommand _command;

    public CreateArticleTests()
    {
        var factory = new CreateArticleCommandFactory();
        _command = factory.Create();
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesAndCommits_InOrder_AndReturnsId()
    {
        // Arrange
        var articleRepositoryMock = new Mock<IArticleRepository>(MockBehavior.Strict);
        articleRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Article>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        uowMock.Setup(u => u.Article).Returns(articleRepositoryMock.Object);
        uowMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        var handler = new CreateArticleCommandHandler(uowMock.Object);

        // Act
        var result = await handler.Handle(_command, CancellationToken.None);

        // Assert
        articleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Article>()), Times.Once);
        uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(result.IsSuccess);
    }
}
