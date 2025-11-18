using Vermundo.Domain.Abstractions;

namespace Vermundo.Domain.Newsletters;

public static class NewsletterSubscriberErrors 
{
    public static readonly Error NewsletterSubscriberNotFound = new Error(
        "NewsletterSubscriber.NotFound", 
        "Newsletter subscriber not found");
    
    public static readonly Error InvalidConfirmationToken = new Error(
        "NewsletterSubscriber.InvalidToken", 
        "Invalid confirmation token");
}
