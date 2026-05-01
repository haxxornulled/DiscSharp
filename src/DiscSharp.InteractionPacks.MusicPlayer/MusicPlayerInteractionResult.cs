namespace DiscSharp.InteractionPacks.MusicPlayer;

/// <summary>
/// Represents the result of a Music Player interaction operation.
/// </summary>
public sealed record MusicPlayerInteractionResult(bool Success, string Message, bool Ephemeral = false)
{
    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static MusicPlayerInteractionResult Ok(string message, bool ephemeral = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new MusicPlayerInteractionResult(true, message, ephemeral);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static MusicPlayerInteractionResult Fail(string message, bool ephemeral = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new MusicPlayerInteractionResult(false, message, ephemeral);
    }
}
