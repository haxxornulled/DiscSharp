using System.Text.Json.Serialization;

namespace DiscSharp.Rest.RateLimits;

/// <summary>
/// Represents Discord's HTTP 429 JSON response body.
/// </summary>
public sealed record DiscordRateLimitResponse
{
    /// <summary>
    /// Gets the Discord response message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of seconds to wait before retrying.
    /// </summary>
    [JsonPropertyName("retry_after")]
    public double RetryAfterSeconds { get; init; }

    /// <summary>
    /// Gets a value indicating whether the rate limit is global.
    /// </summary>
    [JsonPropertyName("global")]
    public bool Global { get; init; }

    /// <summary>
    /// Gets the optional Discord error code.
    /// </summary>
    [JsonPropertyName("code")]
    public int? Code { get; init; }

    /// <summary>
    /// Gets the retry-after delay.
    /// </summary>
    [JsonIgnore]
    public TimeSpan RetryAfter => TimeSpan.FromSeconds(RetryAfterSeconds);
}
