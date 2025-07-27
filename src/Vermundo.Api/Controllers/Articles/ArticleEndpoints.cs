using MediatR;
using Vermundo.Application.Articles;

namespace Vermundo.Api.Controllers.Articles;

public static class ArticleEndpoints
{
    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("articles/{id:guid}", GetArticle).WithName(nameof(GetArticle));

        builder.MapPost("articles", AddArticle);
        
        return builder;
    }

    public static async Task<IResult> GetArticle(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public static async Task<IResult> AddArticle(
        CreateArticleRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateArticleCommand(request.Title, request.Body);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.CreatedAtRoute(nameof(GetArticle), new { id = result.Value }, result.Value);
    }
}
