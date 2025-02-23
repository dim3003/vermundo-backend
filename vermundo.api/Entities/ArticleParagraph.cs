namespace Vermundo.Api.Entities;

public class ArticleParagraph
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int ParagraphIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<ArticleImage> Images { get; set; } = new();
}
