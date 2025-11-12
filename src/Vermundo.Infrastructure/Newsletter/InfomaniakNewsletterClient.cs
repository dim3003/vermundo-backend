using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vermundo.Infrastructure.Newsletter;

public class InfomaniakNewsletterClient : INewsletterClient
{
    private readonly HttpClient _httpClient;
    private readonly InfomaniakNewsletterOptions _options;
    private readonly ILogger<InfomaniakNewsletterClient> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public InfomaniakNewsletterClient(
        HttpClient httpClient,
        IOptions<InfomaniakNewsletterOptions> options,
        ILogger<InfomaniakNewsletterClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email must not be empty.", nameof(email));

        var url = $"newsletters/{_options.Domain}/subscribers";

        var payload = new
        {
            email
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(url, content, cancellationToken);

        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogWarning(
            "Infomaniak subscribe failed for {Email}. StatusCode: {StatusCode}, Body: {Body}",
            email,
            (int)response.StatusCode,
            body);

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new InvalidOperationException($"Infomaniak rejected the request: {body}");

            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
                throw new InvalidOperationException("Infomaniak authentication failed. Check API token / permissions.");

            case HttpStatusCode.TooManyRequests:
                throw new InvalidOperationException("Infomaniak rate limit hit.");

            default:
                throw new InvalidOperationException(
                    $"Infomaniak subscribe failed with status {(int)response.StatusCode}: {body}");
        }
    }
}
