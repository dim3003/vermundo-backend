using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Articles;

public class Article : Entity
{
    private Article() { }

    public Article(string title, string body)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Title = title;
        Body = body;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
