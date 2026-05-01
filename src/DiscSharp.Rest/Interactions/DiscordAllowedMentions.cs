using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Controls which mentions are allowed to ping users, roles, or everyone.
/// </summary>
public sealed record DiscordAllowedMentions
{
    /// <summary>Gets mention parse modes.</summary>
    [JsonPropertyName("parse")]
    public IReadOnlyList<string> Parse { get; init; } = [];

    /// <summary>Gets allowed role mentions.</summary>
    [JsonPropertyName("roles")]
    public IReadOnlyList<string>? Roles { get; init; }

    /// <summary>Gets allowed user mentions.</summary>
    [JsonPropertyName("users")]
    public IReadOnlyList<string>? Users { get; init; }

    /// <summary>Gets whether replied users are mentioned.</summary>
    [JsonPropertyName("replied_user")]
    public bool? RepliedUser { get; init; }

    /// <summary>Creates an allowed-mentions object that prevents accidental pings.</summary>
    public static DiscordAllowedMentions None() => new() { Parse = [] };
}
