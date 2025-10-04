using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Articles;

public sealed record DeleteArticleCommand(Guid Id) : ICommand;
