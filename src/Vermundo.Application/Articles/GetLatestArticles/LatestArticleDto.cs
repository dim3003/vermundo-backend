namespace Vermundo.Application.Articles;

public class LatestArticleDto
{
    public DateTime CreatedAt { get; init; }
    public string Title { get; init; } = string.Empty;
    public string BodyPreview { get; init; } = string.Empty;
    public string? ImageUrl { get; init; }
}