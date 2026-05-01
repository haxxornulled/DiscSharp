namespace DiscSharp.InteractionPacks.MusicPlayer;

/// <summary>
/// Represents a music player interaction request.
/// </summary>
public sealed record MusicInteractionRequest(
    string InteractionId,
    string? GuildId,
    string? ChannelId,
    string? UserId,
    string? Query,
    IReadOnlyDictionary<string, string> Arguments);
