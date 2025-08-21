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
            var body = faker.Lorem.Paragraphs(3, "\n\n");
            var image = faker.Image.PicsumUrl();
            return new CreateArticleCommand(title + $" #{i + 1}", body, image);
        }).ToArray();

        foreach (var cmd in payloads)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await sender.Send(cmd, cancellationToken);
        }
    }
}

