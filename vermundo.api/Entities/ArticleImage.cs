namespace Vermundo.Api.Entities;

public class ArticleImage
{
    public int Id { get; set; }
    public int ParagraphId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int? PositionInParagraph { get; set; } 
    public ArticleParagraph Paragraph { get; set; } = null!;
}
