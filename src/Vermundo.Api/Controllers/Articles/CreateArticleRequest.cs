namespace Vermundo.Api.Controllers.Articles;

public record CreateArticleRequest(
    string Title,
    string Body
);
