using System.Text.Json.Serialization;
using DiscSharp.Rest.Components;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents a Discord interaction response payload.
/// </summary>
public sealed record DiscordInteractionResponsePayload
{
    /// <summary>Gets the callback type.</summary>
    [JsonPropertyName("type")]
    public DiscordInteractionCallbackType Type { get; init; }

    /// <summary>Gets callback data.</summary>
    [JsonPropertyName("data")]
    public DiscordInteractionCallbackData? Data { get; init; }

    /// <summary>Creates a PONG response.</summary>
    public static DiscordInteractionResponsePayload Pong() =>
        new() { Type = DiscordInteractionCallbackType.Pong };

    /// <summary>Creates an immediate message response.</summary>
    public static DiscordInteractionResponsePayload Message(string content, bool ephemeral = false, IReadOnlyList<DiscordComponent>? components = null) =>
        new()
        {
            Type = DiscordInteractionCallbackType.ChannelMessageWithSource,
            Data = DiscordInteractionCallbackData.Message(content, ephemeral ? DiscordMessageFlags.Ephemeral : DiscordMessageFlags.None, components)
        };

    /// <summary>Creates a deferred channel message response.</summary>
    public static DiscordInteractionResponsePayload DeferChannelMessage(bool ephemeral = false) =>
        new()
        {
            Type = DiscordInteractionCallbackType.DeferredChannelMessageWithSource,
            Data = ephemeral ? new DiscordInteractionCallbackData { Flags = (int)DiscordMessageFlags.Ephemeral } : null
        };

    /// <summary>Creates a deferred message update response for component interactions.</summary>
    public static DiscordInteractionResponsePayload DeferUpdateMessage() =>
        new() { Type = DiscordInteractionCallbackType.DeferredUpdateMessage };

    /// <summary>Creates an update-message response for component interactions.</summary>
    public static DiscordInteractionResponsePayload UpdateMessage(string content, IReadOnlyList<DiscordComponent>? components = null) =>
        new()
        {
            Type = DiscordInteractionCallbackType.UpdateMessage,
            Data = DiscordInteractionCallbackData.Message(content, DiscordMessageFlags.None, components)
        };

    /// <summary>Creates a modal response.</summary>
    public static DiscordInteractionResponsePayload Modal(string customId, string title, IReadOnlyList<DiscordComponent> components) =>
        new()
        {
            Type = DiscordInteractionCallbackType.Modal,
            Data = DiscordInteractionCallbackData.Modal(customId, title, components)
        };

    /// <summary>Creates an autocomplete response.</summary>
    public static DiscordInteractionResponsePayload Autocomplete(IReadOnlyList<DiscordApplicationCommandOptionChoice> choices) =>
        new()
        {
            Type = DiscordInteractionCallbackType.ApplicationCommandAutocompleteResult,
            Data = DiscordInteractionCallbackData.Autocomplete(choices)
        };
}
