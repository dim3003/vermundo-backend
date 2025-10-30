using MediatR;
using Vermundo.Application.Articles;

namespace Vermundo.Api.Controllers.Articles;

public static class ArticleEndpoints
{
    public static IEndpointRouteBuilder MapArticleEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("articles/{id:guid}", GetArticle).WithName(nameof(GetArticle));
        builder.MapGet("articles/latest", GetLatestArticles).WithName(nameof(GetLatestArticles));

        builder.MapPost("articles", AddArticle);

        builder.MapPut("articles/{id:guid}", UpdateArticle);

        builder.MapDelete("articles/{id:guid}", DeleteArticle).WithName(nameof(DeleteArticle));

        return builder;
    }

    public static async Task<IResult> GetArticle(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetArticleQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.NotFound(result.Error);
    }

    public static async Task<IResult> GetLatestArticles(
            ISender sender, 
            CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetLatestArticlesQuery(), cancellationToken);

        return result.IsSuccess 
            ? Results.Ok(result.Value)
            : Results.NotFound();
    }

    public static async Task<IResult> AddArticle(
        CreateArticleRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateArticleCommand(request.Title, request.Body, request.ImageUrl);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.CreatedAtRoute(nameof(GetArticle), new { id = result.Value }, result.Value);
    }

    public static async Task<IResult> UpdateArticle(
        Guid id,
        UpdateArticleRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new UpdateArticleCommand(id, request.ImageUrl);
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.NotFound(result.Error);
    }

    public static async Task<IResult> DeleteArticle(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new DeleteArticleCommand(id);
        var result = await sender.Send(query, cancellationToken);
        return result.IsSuccess 
            ? Results.NoContent() 
            : Results.NotFound(result.Error);
    }
}
