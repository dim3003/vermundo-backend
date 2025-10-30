using Vermundo.Application.Abstractions.Messaging;

public record UpdateArticleCommand(Guid Id, string ImageUrl) : ICommand<Guid>;
