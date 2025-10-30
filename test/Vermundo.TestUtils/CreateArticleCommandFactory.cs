using Vermundo.Application.Articles;

namespace Vermundo.TestUtils;

public class CreateArticleCommandFactory
{
    public CreateArticleCommand Create(string title, string body, string? imageUrl = null)
    {
        return new CreateArticleCommand(title, body, imageUrl);
    }
}
