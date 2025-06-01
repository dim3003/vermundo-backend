namespace Vermundo.Domain.Articles;

public class ArticleParagraph
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int ParagraphIndex { get; set; }
    public string Text { get; set; } = string.Empty;
}
