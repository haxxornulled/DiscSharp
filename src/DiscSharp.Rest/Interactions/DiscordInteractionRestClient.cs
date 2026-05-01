using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using DiscSharp.Rest.Errors;
using DiscSharp.Rest.Http;
using DiscSharp.Rest.Primitives;
using DiscSharp.Rest.RateLimits;
using DiscSharp.Rest.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Default HTTP implementation of <see cref="IDiscordInteractionRestClient"/>.
/// </summary>
public sealed class DiscordInteractionRestClient : IDiscordInteractionRestClient
{
    private readonly HttpClient _httpClient;
    private readonly DiscordApiOptions _options;
    private readonly DiscordRestTelemetry _telemetry;
    private readonly ILogger<DiscordInteractionRestClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordInteractionRestClient"/> class.
    /// </summary>
    public DiscordInteractionRestClient(
        HttpClient httpClient,
        IOptions<DiscordApiOptions> options,
        DiscordRestTelemetry telemetry,
        ILogger<DiscordInteractionRestClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options.Validate();
    }

    /// <summary>
    /// Creates the initial interaction callback response for the supplied interaction token.
    /// </summary>
    public async ValueTask<DiscordInteractionCallbackResponse?> CreateInteractionResponseAsync(
        DiscordSnowflake interactionId,
        string interactionToken,
        DiscordInteractionResponsePayload payload,
        bool withResponse = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        ArgumentNullException.ThrowIfNull(payload);

        var query = new DiscordQueryStringBuilder()
            .AddBoolean("with_response", withResponse ? true : null)
            .Build();

        var route = DiscordApiRoutes.CreateInteractionResponse(interactionId, interactionToken);
        return await SendJsonAsync<DiscordInteractionResponsePayload, DiscordInteractionCallbackResponse>(route, payload, query, expectBody: withResponse, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the original interaction response message.
    /// </summary>
    public async ValueTask<DiscordWebhookMessage> GetOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        return await SendAsync<DiscordWebhookMessage>(DiscordApiRoutes.GetOriginalInteractionResponse(applicationId, interactionToken), null, expectBody: true, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Discord returned an empty original interaction response.");
    }

    /// <summary>
    /// Edits the original interaction response message.
    /// </summary>
    public async ValueTask<DiscordWebhookMessage> EditOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        ArgumentNullException.ThrowIfNull(payload);
        return await SendJsonAsync<DiscordWebhookMessagePayload, DiscordWebhookMessage>(DiscordApiRoutes.EditOriginalInteractionResponse(applicationId, interactionToken), payload, null, expectBody: true, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Discord returned an empty edited original interaction response.");
    }

    /// <summary>
    /// Deletes the original interaction response message.
    /// </summary>
    public async ValueTask DeleteOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        await SendAsync<object>(DiscordApiRoutes.DeleteOriginalInteractionResponse(applicationId, interactionToken), null, expectBody: false, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a followup message for the supplied interaction.
    /// </summary>
    public async ValueTask<DiscordWebhookMessage> CreateFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        ArgumentNullException.ThrowIfNull(payload);
        return await SendJsonAsync<DiscordWebhookMessagePayload, DiscordWebhookMessage>(DiscordApiRoutes.CreateInteractionFollowup(applicationId, interactionToken), payload, null, expectBody: true, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Discord returned an empty followup response.");
    }

    /// <summary>
    /// Retrieves a followup message by message ID.
    /// </summary>
    public async ValueTask<DiscordWebhookMessage> GetFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        return await SendAsync<DiscordWebhookMessage>(DiscordApiRoutes.GetInteractionFollowup(applicationId, interactionToken, messageId), null, expectBody: true, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Discord returned an empty followup response.");
    }

    /// <summary>
    /// Edits a followup message by message ID.
    /// </summary>
    public async ValueTask<DiscordWebhookMessage> EditFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        ArgumentNullException.ThrowIfNull(payload);
        return await SendJsonAsync<DiscordWebhookMessagePayload, DiscordWebhookMessage>(DiscordApiRoutes.EditInteractionFollowup(applicationId, interactionToken, messageId), payload, null, expectBody: true, cancellationToken)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Discord returned an empty edited followup response.");
    }

    /// <summary>
    /// Deletes a followup message by message ID.
    /// </summary>
    public async ValueTask DeleteFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionToken);
        await SendAsync<object>(DiscordApiRoutes.DeleteInteractionFollowup(applicationId, interactionToken, messageId), null, expectBody: false, cancellationToken)
            .ConfigureAwait(false);
    }

    private ValueTask<TResponse?> SendJsonAsync<TRequest, TResponse>(
        DiscordApiRoute route,
        TRequest payload,
        string? queryString,
        bool expectBody,
        CancellationToken cancellationToken)
        where TRequest : class
    {
        var json = JsonSerializer.Serialize(payload, DiscordRestJson.Options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return SendAsync<TResponse>(route, content, expectBody, cancellationToken, queryString);
    }

    private async ValueTask<TResponse?> SendAsync<TResponse>(
        DiscordApiRoute route,
        HttpContent? content,
        bool expectBody,
        CancellationToken cancellationToken,
        string? queryString = null)
    {
        ArgumentNullException.ThrowIfNull(route);
        var uri = route.BuildUri(_options, queryString);
        using var activity = _telemetry.ActivitySource.StartActivity("Discord REST", ActivityKind.Client);
        activity?.SetTag("http.request.method", route.Method.Method);
        activity?.SetTag("url.path", route.Path);

        using var request = new HttpRequestMessage(route.Method, uri) { Content = content };
        request.Headers.UserAgent.ParseAdd(_options.UserAgent);
        _telemetry.Requests.Add(1, new KeyValuePair<string, object?>("discord.route", route.Path));

        _logger.LogDebug("Sending Discord REST request {Method} {Route}", route.Method, route.Path);
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        var rateLimitHeaders = DiscordRateLimitHeaders.From(response);
        activity?.SetTag("http.response.status_code", (int)response.StatusCode);
        activity?.SetTag("discord.ratelimit.bucket", rateLimitHeaders.Bucket);
        activity?.SetTag("discord.ratelimit.remaining", rateLimitHeaders.Remaining);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _telemetry.RateLimited.Add(1, new KeyValuePair<string, object?>("discord.route", route.Path));
        }

        if (!response.IsSuccessStatusCode)
        {
            _telemetry.Failures.Add(1, new KeyValuePair<string, object?>("discord.route", route.Path));
            var error = await ReadErrorAsync(response, cancellationToken).ConfigureAwait(false);
            _logger.LogWarning(
                "Discord REST request {Method} {Route} failed with {StatusCode}. Bucket={Bucket} Scope={Scope} RetryAfter={RetryAfter}",
                route.Method,
                route.Path,
                response.StatusCode,
                rateLimitHeaders.Bucket,
                rateLimitHeaders.Scope,
                rateLimitHeaders.RetryAfter);
            throw new DiscordRestException(response.StatusCode, route.ToString(), error, rateLimitHeaders);
        }

        if (!expectBody || response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<TResponse>(stream, DiscordRestJson.Options, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<DiscordApiError?> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content is null)
        {
            return null;
        }

        try
        {
            if (response.Content.Headers.ContentLength == 0)
            {
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<DiscordApiError>(stream, DiscordRestJson.Options, cancellationToken).ConfigureAwait(false);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
