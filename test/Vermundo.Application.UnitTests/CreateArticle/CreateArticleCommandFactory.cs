using Bogus;
using Vermundo.Application.Articles;

namespace Vermundo.Application.UnitTests.CreateArticle;

public class CreateArticleCommandFactory
{
    private readonly Faker _faker = new();

    public CreateArticleCommand Create()
    {
        return new CreateArticleCommand(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraphs(2)
        );
    }
}

