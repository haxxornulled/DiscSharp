namespace DiscSharp.InteractionPacks.RaidManager;

/// <summary>
/// Represents a raid interaction request.
/// </summary>
public sealed record RaidInteractionRequest(
    string InteractionId,
    string? GuildId,
    string? ChannelId,
    string? UserId,
    string RaidId,
    string? Role);
