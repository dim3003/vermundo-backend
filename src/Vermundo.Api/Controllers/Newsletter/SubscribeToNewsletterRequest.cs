namespace Vermundo.Api.Controllers.Newsletter;

public sealed record SubscribeToNewsletterRequest(
    [System.ComponentModel.DataAnnotations.EmailAddress]
    [System.ComponentModel.DataAnnotations.Required]
    string Email);

