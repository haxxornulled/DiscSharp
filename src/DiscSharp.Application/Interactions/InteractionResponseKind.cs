namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents a Discord interaction response kind at application level.
/// </summary>
public enum InteractionResponseKind
{
    /// <summary>
    /// No response should be written.
    /// </summary>
    None = 0,

    /// <summary>
    /// Send an immediate channel message response.
    /// </summary>
    ChannelMessage = 1,

    /// <summary>
    /// Defer the interaction response.
    /// </summary>
    DeferredChannelMessage = 2,

    /// <summary>
    /// Update the original message for component interactions.
    /// </summary>
    UpdateMessage = 3,

    /// <summary>
    /// Defer an update to the original message.
    /// </summary>
    DeferredUpdateMessage = 4,

    /// <summary>
    /// Open a modal response.
    /// </summary>
    Modal = 5
}
