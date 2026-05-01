namespace DiscSharp.Rest.Http;

/// <summary>
/// Provides configuration for Discord REST API clients.
/// </summary>
public sealed class DiscordApiOptions
{
    /// <summary>
    /// Gets or sets the unversioned Discord API base URI.
    /// </summary>
    public Uri BaseUri { get; set; } = new("https://discord.com/api/", UriKind.Absolute);

    /// <summary>
    /// Gets or sets the Discord API version. DiscSharp targets v10 by default.
    /// </summary>
    public int ApiVersion { get; set; } = 10;

    /// <summary>
    /// Gets or sets the User-Agent sent to Discord.
    /// </summary>
    public string UserAgent { get; set; } = "DiscordBot (https://github.com/haxxornulled/DiscSharp, 0.1.0)";

    /// <summary>
    /// Gets or sets the initial interaction response deadline.
    /// </summary>
    public TimeSpan InteractionInitialResponseDeadline { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Gets or sets the usable lifetime of an interaction token.
    /// </summary>
    public TimeSpan InteractionTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets the versioned Discord API base URI.
    /// </summary>
    public Uri VersionedBaseUri => new(BaseUri, $"v{ApiVersion}/");

    /// <summary>
    /// Validates the current options.
    /// </summary>
    public void Validate()
    {
        if (!BaseUri.IsAbsoluteUri)
        {
            throw new InvalidOperationException("Discord API base URI must be absolute.");
        }

        if (ApiVersion < 10)
        {
            throw new InvalidOperationException("DiscSharp targets Discord API v10 or newer.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(UserAgent);

        if (InteractionInitialResponseDeadline <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Interaction initial response deadline must be positive.");
        }

        if (InteractionTokenLifetime <= InteractionInitialResponseDeadline)
        {
            throw new InvalidOperationException("Interaction token lifetime must be longer than the initial response deadline.");
        }
    }
}
