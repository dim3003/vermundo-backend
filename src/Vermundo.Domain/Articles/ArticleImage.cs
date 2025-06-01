namespace Vermundo.Domain.Articles;

public class ArticleImage
{
    public int Id { get; set; }
    public int ArticleParagraphId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int? PositionInParagraph { get; set; } 
}
