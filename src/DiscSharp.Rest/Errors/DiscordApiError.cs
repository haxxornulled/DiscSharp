using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Errors;

/// <summary>
/// Represents Discord's standard JSON error response.
/// </summary>
public sealed record DiscordApiError
{
    /// <summary>
    /// Gets Discord's numeric error code.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; init; }

    /// <summary>
    /// Gets Discord's human-readable error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets detailed form-body validation errors when supplied by Discord.
    /// </summary>
    [JsonPropertyName("errors")]
    public JsonElement? Errors { get; init; }
}
