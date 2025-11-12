using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Newsletter;

public record SubscribeToNewsletterCommand(string Email) : ICommand;
