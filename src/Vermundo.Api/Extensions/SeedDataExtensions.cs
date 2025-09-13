using Bogus;
using MediatR;
using Vermundo.Application.Articles;
using Vermundo.Infrastructure;

namespace Vermundo.Api.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedData(this IApplicationBuilder app, CancellationToken cancellationToken = default)
    {
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
        if (!env.IsDevelopment()) return; 

        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        if(!context.Articles.Any())
        {
            await CreateFakeArticles(sender, cancellationToken);
        }
    }

    private static async Task CreateFakeArticles(
            ISender sender,
            CancellationToken cancellationToken)
    {
        var faker = new Faker(locale: "fr_CH");

        var payloads = Enumerable.Range(0, 5).Select(i =>
        {
            var title = faker.Lorem.Sentence(5);
            var paragraph1= "## Subtitle\n\n" 
                + faker.Lorem.Sentence(6);
            var paragraph2= "## Subtitle\n\n" 
                + faker.Lorem.Sentence(13) 
                + "\n\n"
                + faker.Lorem.Sentence(5)
                + "\n\n";
            var sources = @"## Source\n\n
                - [Example source title — Example source author](https://example.com)";

            var fullBody = paragraph1 + paragraph2 + sources;

            var image = faker.Image.PicsumUrl();
            return new CreateArticleCommand(title + $" #{i + 1}", fullBody, image);
        }).ToArray();


        foreach (var cmd in payloads)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await sender.Send(cmd, cancellationToken);
        }
    }
}

