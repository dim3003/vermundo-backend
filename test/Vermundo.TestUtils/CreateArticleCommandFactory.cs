using Bogus;
using Vermundo.Application.Articles;

namespace Vermundo.TestUtils;

public class CreateArticleCommandFactory
{
    private readonly Faker _faker = new();

    public CreateArticleCommand Create()
    {
        return new CreateArticleCommand(_faker.Lorem.Sentence(), _faker.Lorem.Paragraphs(2));
    }

    public CreateArticleCommand Create(string title, string body, string? imageUrl = null)
    {
        return new CreateArticleCommand(title, body, imageUrl);
    }
}
