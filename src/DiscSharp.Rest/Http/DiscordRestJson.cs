using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Http;

/// <summary>
/// Provides JSON serialization settings for Discord REST payloads.
/// </summary>
public static class DiscordRestJson
{
    /// <summary>
    /// Gets the default JSON serializer options used by DiscSharp REST clients.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
    }
}
