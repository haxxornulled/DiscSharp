using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents Discord's optional interaction callback response returned when with_response=true.
/// </summary>
public sealed record DiscordInteractionCallbackResponse
{
    /// <summary>Gets the callback interaction data when supplied.</summary>
    [JsonPropertyName("interaction")]
    public JsonElement? Interaction { get; init; }

    /// <summary>Gets the created response resource when supplied.</summary>
    [JsonPropertyName("resource")]
    public JsonElement? Resource { get; init; }
}
