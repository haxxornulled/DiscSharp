namespace DiscSharp.Application.Interactions;

/// <summary>
/// Describes the response that should be written for a Discord interaction.
/// </summary>
public sealed record InteractionResponsePlan
{
    private InteractionResponsePlan(
        InteractionResponseKind kind,
        string? content,
        bool ephemeral,
        string? modalCustomId,
        string? modalTitle)
    {
        Kind = kind;
        Content = content;
        Ephemeral = ephemeral;
        ModalCustomId = modalCustomId;
        ModalTitle = modalTitle;
    }

    /// <summary>
    /// Gets the response kind.
    /// </summary>
    public InteractionResponseKind Kind { get; }

    /// <summary>
    /// Gets the response content, when applicable.
    /// </summary>
    public string? Content { get; }

    /// <summary>
    /// Gets a value indicating whether the response should be ephemeral.
    /// </summary>
    public bool Ephemeral { get; }

    /// <summary>
    /// Gets the modal custom ID, when applicable.
    /// </summary>
    public string? ModalCustomId { get; }

    /// <summary>
    /// Gets the modal title, when applicable.
    /// </summary>
    public string? ModalTitle { get; }

    /// <summary>
    /// Creates a no-op response plan.
    /// </summary>
    public static InteractionResponsePlan None() => new(InteractionResponseKind.None, null, ephemeral: false, null, null);

    /// <summary>
    /// Creates an immediate channel message response plan.
    /// </summary>
    public static InteractionResponsePlan Message(string content, bool ephemeral = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new InteractionResponsePlan(InteractionResponseKind.ChannelMessage, content, ephemeral, null, null);
    }

    /// <summary>
    /// Creates a deferred channel message response plan.
    /// </summary>
    public static InteractionResponsePlan Defer(bool ephemeral = false) =>
        new(InteractionResponseKind.DeferredChannelMessage, null, ephemeral, null, null);

    /// <summary>
    /// Creates a message update response plan.
    /// </summary>
    public static InteractionResponsePlan UpdateMessage(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new InteractionResponsePlan(InteractionResponseKind.UpdateMessage, content, ephemeral: false, null, null);
    }

    /// <summary>
    /// Creates a deferred update response plan.
    /// </summary>
    public static InteractionResponsePlan DeferUpdate() =>
        new(InteractionResponseKind.DeferredUpdateMessage, null, ephemeral: false, null, null);

    /// <summary>
    /// Creates a modal response plan.
    /// </summary>
    public static InteractionResponsePlan Modal(string customId, string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        return new InteractionResponsePlan(InteractionResponseKind.Modal, null, ephemeral: false, customId, title);
    }
}
