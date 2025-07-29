using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Articles;

public class Article : Entity
{
    private Article() { }

    public Article(
        string title,
        string body,
        string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Title = title;
        Body = body;
        ImageUrl = imageUrl;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
