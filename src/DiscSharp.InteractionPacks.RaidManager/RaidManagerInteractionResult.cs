namespace DiscSharp.InteractionPacks.RaidManager;

/// <summary>
/// Represents the result of a Raid Manager interaction operation.
/// </summary>
public sealed record RaidManagerInteractionResult(bool Success, string Message, bool Ephemeral = true)
{
    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static RaidManagerInteractionResult Ok(string message, bool ephemeral = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new RaidManagerInteractionResult(true, message, ephemeral);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static RaidManagerInteractionResult Fail(string message, bool ephemeral = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new RaidManagerInteractionResult(false, message, ephemeral);
    }
}
