using System.Net;
using DiscSharp.Rest.RateLimits;

namespace DiscSharp.Rest.Errors;

/// <summary>
/// Represents a non-success Discord REST response.
/// </summary>
public sealed class DiscordRestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestException"/> class.
    /// </summary>
    public DiscordRestException(
        HttpStatusCode statusCode,
        string route,
        DiscordApiError? error,
        DiscordRateLimitHeaders rateLimitHeaders)
        : base(BuildMessage(statusCode, route, error))
    {
        StatusCode = statusCode;
        Route = route;
        Error = error;
        RateLimitHeaders = rateLimitHeaders;
    }

    /// <summary>
    /// Gets the HTTP response status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the REST route that failed.
    /// </summary>
    public string Route { get; }

    /// <summary>
    /// Gets Discord's parsed error body when available.
    /// </summary>
    public DiscordApiError? Error { get; }

    /// <summary>
    /// Gets Discord rate-limit metadata returned with the response.
    /// </summary>
    public DiscordRateLimitHeaders RateLimitHeaders { get; }

    private static string BuildMessage(HttpStatusCode statusCode, string route, DiscordApiError? error)
    {
        var status = (int)statusCode;
        var prefix = $"Discord REST request failed with HTTP {status} ({statusCode}) for {route}";
        return error is null ? prefix : $"{prefix}: {error.Code} {error.Message}";
    }
}
