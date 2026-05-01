namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents an application-facing Discord interaction independent of transport DTOs.
/// </summary>
public sealed record DiscordInteractionEnvelope
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordInteractionEnvelope"/> class.
    /// </summary>
    public DiscordInteractionEnvelope(
        string interactionId,
        string applicationId,
        DiscordInteractionKind kind,
        string? token,
        string? guildId,
        string? channelId,
        string? userId,
        string? commandName,
        string? customId,
        object rawPayload,
        DateTimeOffset receivedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(interactionId);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId);
        ArgumentNullException.ThrowIfNull(rawPayload);

        InteractionId = interactionId;
        ApplicationId = applicationId;
        Kind = kind;
        Token = token;
        GuildId = guildId;
        ChannelId = channelId;
        UserId = userId;
        CommandName = commandName;
        CustomId = customId;
        RawPayload = rawPayload;
        ReceivedAt = receivedAt;
    }

    /// <summary>
    /// Gets the Discord interaction identifier.
    /// </summary>
    public string InteractionId { get; }

    /// <summary>
    /// Gets the Discord application identifier.
    /// </summary>
    public string ApplicationId { get; }

    /// <summary>
    /// Gets the interaction kind.
    /// </summary>
    public DiscordInteractionKind Kind { get; }

    /// <summary>
    /// Gets the interaction token, when available.
    /// </summary>
    public string? Token { get; }

    /// <summary>
    /// Gets the guild identifier, when available.
    /// </summary>
    public string? GuildId { get; }

    /// <summary>
    /// Gets the channel identifier, when available.
    /// </summary>
    public string? ChannelId { get; }

    /// <summary>
    /// Gets the user identifier, when available.
    /// </summary>
    public string? UserId { get; }

    /// <summary>
    /// Gets the command name for command interactions.
    /// </summary>
    public string? CommandName { get; }

    /// <summary>
    /// Gets the component or modal custom ID.
    /// </summary>
    public string? CustomId { get; }

    /// <summary>
    /// Gets the raw typed payload used to build this envelope.
    /// </summary>
    public object RawPayload { get; }

    /// <summary>
    /// Gets when the interaction entered the application pipeline.
    /// </summary>
    public DateTimeOffset ReceivedAt { get; }
}
