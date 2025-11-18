using Vermundo.Domain.Abstractions;

namespace Vermundo.Application.Abstractions;

public interface INewsletterSubscriptionService
{
    Task<Result> SubscribeAsync(string rawEmail, CancellationToken ct = default);
}
