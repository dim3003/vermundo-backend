using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Articles;

public sealed record CreateArticleCommand(string Title, string Body) : ICommand;