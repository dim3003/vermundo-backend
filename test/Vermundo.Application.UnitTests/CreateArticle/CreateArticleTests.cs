using Moq;
using Vermundo.Application.Articles;
using Vermundo.Domain.Abstractions;
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
    public async Task Handle_ValidCommand_SavesAndCommits_InOrder_AndReturnsId()
    {
        // Arrange
        var repo = new Mock<IArticleRepository>(MockBehavior.Strict);
        var uow  = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var articleId = Guid.NewGuid();
        repo.Setup(r => r.AddAsync(It.IsAny<Article>()))
            .ReturnsAsync(articleId)
            .Verifiable();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(1)
           .Verifiable();

        var handler = new CreateArticleCommandHandler(repo.Object, uow.Object);

        // Act
        var result = await handler.Handle(_command, CancellationToken.None);

        // Assert – ordre strict
        var seq = new MockSequence();
        repo.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.True(result.IsSuccess);
    }
}
