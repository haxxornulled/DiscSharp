using System.Text.Json.Serialization;
using DiscSharp.Rest.Components;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents payload data used for editing original responses and creating or editing followup messages.
/// </summary>
public sealed record DiscordWebhookMessagePayload
{
    /// <summary>Gets message content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>Gets message flags.</summary>
    [JsonPropertyName("flags")]
    public int? Flags { get; init; }

    /// <summary>Gets allowed mentions.</summary>
    [JsonPropertyName("allowed_mentions")]
    public DiscordAllowedMentions? AllowedMentions { get; init; }

    /// <summary>Gets components.</summary>
    [JsonPropertyName("components")]
    public IReadOnlyList<DiscordComponent>? Components { get; init; }

    /// <summary>Creates a webhook message payload.</summary>
    public static DiscordWebhookMessagePayload Message(string content, DiscordMessageFlags flags = DiscordMessageFlags.None, IReadOnlyList<DiscordComponent>? components = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new DiscordWebhookMessagePayload
        {
            Content = content,
            Flags = flags == DiscordMessageFlags.None ? null : (int)flags,
            Components = components,
            AllowedMentions = DiscordAllowedMentions.None()
        };
    }
}
