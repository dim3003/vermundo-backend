namespace Vermundo.Api.Entities;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ArticleParagraph> Paragraphs { get; set; } = new();
}
