using Microsoft.EntityFrameworkCore;
using Vermundo.Api.Entities;

namespace Vermundo.Api.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleParagraph> ArticleParagraphs { get; set; }
    public DbSet<ArticleBulletList> ArticleBulletLists { get; set; }
    public DbSet<ArticleBulletPoint> ArticleBulletPoints { get; set; }
    public DbSet<ArticleImage> ArticleImages { get; set; }
}
