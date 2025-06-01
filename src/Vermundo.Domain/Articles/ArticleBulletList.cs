namespace Vermundo.Domain.Articles;

public class ArticleBulletList
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public int ListIndex { get; set; }  
    public List<ArticleBulletPoint> BulletPoints { get; set; } = new();
}
