using Vermundo.Application.Abstractions.Messaging;

namespace Vermundo.Application.Newsletter;

public record ConfirmNewsletterSubscriptionCommand(string Token) : ICommand;
