using System.Globalization;
using System.Net.Http.Headers;

namespace DiscSharp.Rest.RateLimits;

/// <summary>
/// Captures Discord rate-limit response headers for routing, retry, and telemetry decisions.
/// </summary>
public sealed record DiscordRateLimitHeaders
{
    /// <summary>
    /// Gets the Discord route bucket capacity when supplied.
    /// </summary>
    public int? Limit { get; init; }

    /// <summary>
    /// Gets the remaining bucket capacity when supplied.
    /// </summary>
    public int? Remaining { get; init; }

    /// <summary>
    /// Gets the UTC reset timestamp when supplied.
    /// </summary>
    public DateTimeOffset? Reset { get; init; }

    /// <summary>
    /// Gets the reset-after delay when supplied.
    /// </summary>
    public TimeSpan? ResetAfter { get; init; }

    /// <summary>
    /// Gets the Discord bucket identifier when supplied.
    /// </summary>
    public string? Bucket { get; init; }

    /// <summary>
    /// Gets a value indicating whether the response is for a global rate limit.
    /// </summary>
    public bool IsGlobal { get; init; }

    /// <summary>
    /// Gets the Discord rate-limit scope when supplied.
    /// </summary>
    public DiscordRateLimitScope? Scope { get; init; }

    /// <summary>
    /// Gets the retry-after delay from the standard header when supplied.
    /// </summary>
    public TimeSpan? RetryAfter { get; init; }

    /// <summary>
    /// Parses Discord rate-limit headers from an HTTP response.
    /// </summary>
    public static DiscordRateLimitHeaders From(HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return From(response.Headers, response.Content?.Headers);
    }

    /// <summary>
    /// Parses Discord rate-limit headers from HTTP header collections.
    /// </summary>
    public static DiscordRateLimitHeaders From(HttpResponseHeaders headers, HttpContentHeaders? contentHeaders = null)
    {
        ArgumentNullException.ThrowIfNull(headers);

        return new DiscordRateLimitHeaders
        {
            Limit = TryGetInt(headers, "X-RateLimit-Limit"),
            Remaining = TryGetInt(headers, "X-RateLimit-Remaining"),
            Reset = TryGetUnixTime(headers, "X-RateLimit-Reset"),
            ResetAfter = TryGetSeconds(headers, "X-RateLimit-Reset-After"),
            Bucket = TryGetString(headers, "X-RateLimit-Bucket"),
            IsGlobal = TryGetBool(headers, "X-RateLimit-Global"),
            Scope = TryGetScope(headers, "X-RateLimit-Scope"),
            RetryAfter = TryGetRetryAfter(headers) ?? TryGetSeconds(contentHeaders, "Retry-After")
        };
    }

    private static string? TryGetString(HttpHeaders? headers, string name)
    {
        return headers is not null && headers.TryGetValues(name, out var values)
            ? values.FirstOrDefault()
            : null;
    }

    private static int? TryGetInt(HttpHeaders? headers, string name)
    {
        var value = TryGetString(headers, name);
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    }

    private static bool TryGetBool(HttpHeaders? headers, string name)
    {
        var value = TryGetString(headers, name);
        return bool.TryParse(value, out var parsed) && parsed;
    }

    private static DateTimeOffset? TryGetUnixTime(HttpHeaders? headers, string name)
    {
        var value = TryGetString(headers, name);
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
        {
            return null;
        }

        var milliseconds = checked((long)Math.Round(seconds * 1000d, MidpointRounding.AwayFromZero));
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }

    private static TimeSpan? TryGetSeconds(HttpHeaders? headers, string name)
    {
        var value = TryGetString(headers, name);
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)
            ? TimeSpan.FromSeconds(seconds)
            : null;
    }

    private static TimeSpan? TryGetRetryAfter(HttpResponseHeaders headers)
    {
        if (headers.RetryAfter?.Delta is { } delta)
        {
            return delta;
        }

        if (headers.RetryAfter?.Date is { } date)
        {
            var delay = date - DateTimeOffset.UtcNow;
            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        return TryGetSeconds(headers, "Retry-After");
    }

    private static DiscordRateLimitScope? TryGetScope(HttpHeaders headers, string name)
    {
        var value = TryGetString(headers, name);
        return value?.ToLowerInvariant() switch
        {
            "user" => DiscordRateLimitScope.User,
            "global" => DiscordRateLimitScope.Global,
            "shared" => DiscordRateLimitScope.Shared,
            _ => null
        };
    }
}
