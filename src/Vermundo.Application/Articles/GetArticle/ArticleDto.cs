namespace Vermundo.Application.Articles;

public class ArticleDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? ImageUrl { get; init; } 
    public string Body { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}