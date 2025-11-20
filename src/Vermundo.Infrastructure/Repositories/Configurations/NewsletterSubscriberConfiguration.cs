using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vermundo.Domain.Newsletters;

public class NewsletterSubscriberConfiguration : IEntityTypeConfiguration<NewsletterSubscriber>
{
    public void Configure(EntityTypeBuilder<NewsletterSubscriber> builder)
    {
        builder.ToTable("newsletter_subscribers");

        builder.HasIndex(s => s.Email).IsUnique();

        builder.Property(s => s.Email)
               .IsRequired()
               .HasMaxLength(320); // max email length
    }
}

