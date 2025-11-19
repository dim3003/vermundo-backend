namespace Vermundo.Api.Controllers.Newsletter;

public sealed record ConfirmNewsletterSubscriptionRequest(
    [System.ComponentModel.DataAnnotations.Required]
    string Token);

