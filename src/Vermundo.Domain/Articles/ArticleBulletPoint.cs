namespace Vermundo.Domain.Articles;

public class ArticleBulletPoint
{
    public int Id { get; set; }
    public int BulletListId { get; set; }
    public int Order { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
