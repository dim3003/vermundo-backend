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
        ILogger<InfomaniakNewsletterClient> logger
    )
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<int> SubscribeAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email must not be empty.", nameof(email));

        var url = $"newsletters/{_options.Domain}/subscribers";

        var payload = new { email };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        // Success path: parse and return the subscriber id
        if (response.IsSuccessStatusCode)
        {
            return ParseSuccessSubscribe(email, body);
        }


        _logger.LogWarning(
            "Infomaniak subscribe failed for {Email}. StatusCode: {StatusCode}, Body: {Body}",
            email,
            (int)response.StatusCode,
            body
        );

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new HttpRequestException(
                    $"Infomaniak rejected the request: {body}",
                    inner: null,
                    statusCode: HttpStatusCode.BadRequest
                );

            case HttpStatusCode.Unauthorized:
                throw new HttpRequestException(
                    "Infomaniak authentication failed. Check API token / permissions.",
                    inner: null,
                    statusCode: HttpStatusCode.Unauthorized
                );

            case HttpStatusCode.Forbidden:
                throw new HttpRequestException(
                    "Infomaniak authentication failed. Check API token / permissions.",
                    inner: null,
                    statusCode: HttpStatusCode.Forbidden
                );

            case HttpStatusCode.TooManyRequests:
                throw new HttpRequestException(
                    "Infomaniak rate limit hit.",
                    inner: null,
                    statusCode: HttpStatusCode.TooManyRequests
                );

            default:
                throw new HttpRequestException(
                    $"Infomaniak subscribe failed with status {(int)response.StatusCode}: {body}",
                    inner: null,
                    statusCode: response.StatusCode
                );
        }
    }

    private int ParseSuccessSubscribe(string email, string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            var id = doc
                .RootElement
                .GetProperty("data")
                .GetProperty("id")
                .GetInt32();

            return id;
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            _logger.LogError(
                ex,
                "Infomaniak response for {Email} did not contain a valid subscriber id. Raw body: {Body}",
                email,
                body
            );

            throw new InvalidOperationException(
                "Infomaniak response did not contain a valid subscriber id.",
                ex
            );
        }
    }

    public async Task ConfirmAsync(int providerId, CancellationToken cancellationToken = default)
    {
        if (providerId <= 0)
            throw new ArgumentOutOfRangeException(nameof(providerId), "Provider id must be positive.");

        var url = $"newsletters/{_options.Domain}/subscribers/{providerId}";

        // Infomaniak “update subscriber” – we only care about confirming (activating)
        var payload = new { status = "active" };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PutAsync(url, content, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        // Success path: nothing more to do for now
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        _logger.LogWarning(
            "Infomaniak confirm failed for subscriber {ProviderId}. StatusCode: {StatusCode}, Body: {Body}",
            providerId,
            (int)response.StatusCode,
            body
        );

        switch (response.StatusCode)
        {
            case HttpStatusCode.BadRequest:
                throw new HttpRequestException(
                    $"Infomaniak rejected the request: {body}",
                    inner: null,
                    statusCode: HttpStatusCode.BadRequest
                );

            case HttpStatusCode.Unauthorized:
                throw new HttpRequestException(
                    "Infomaniak authentication failed. Check API token / permissions.",
                    inner: null,
                    statusCode: HttpStatusCode.Unauthorized
                );

            case HttpStatusCode.Forbidden:
                throw new HttpRequestException(
                    "Infomaniak authentication failed. Check API token / permissions.",
                    inner: null,
                    statusCode: HttpStatusCode.Forbidden
                );

            case HttpStatusCode.TooManyRequests:
                throw new HttpRequestException(
                    "Infomaniak rate limit hit.",
                    inner: null,
                    statusCode: HttpStatusCode.TooManyRequests
                );

            default:
                throw new HttpRequestException(
                    $"Infomaniak confirm failed with status {(int)response.StatusCode}: {body}",
                    inner: null,
                    statusCode: response.StatusCode
                );
        }
    }
}
