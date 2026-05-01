using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents a Discord webhook message response while preserving unmapped fields.
/// </summary>
public sealed record DiscordWebhookMessage
{
    /// <summary>Gets the message ID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>Gets message content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>Gets unmapped Discord message fields.</summary>
    [JsonExtensionData]
    public IDictionary<string, JsonElement> ExtensionData { get; init; } = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
}
