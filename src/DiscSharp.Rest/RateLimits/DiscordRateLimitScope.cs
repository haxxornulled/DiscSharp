namespace DiscSharp.Rest.RateLimits;

/// <summary>
/// Describes the Discord rate limit scope returned on HTTP 429 responses.
/// </summary>
public enum DiscordRateLimitScope
{
    /// <summary>
    /// Per user or per bot token limit.
    /// </summary>
    User,

    /// <summary>
    /// Global per user or per bot token limit.
    /// </summary>
    Global,

    /// <summary>
    /// Shared resource limit.
    /// </summary>
    Shared
}
