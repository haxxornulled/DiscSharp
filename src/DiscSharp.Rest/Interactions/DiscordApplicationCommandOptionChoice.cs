using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents an autocomplete choice returned to Discord.
/// </summary>
public sealed record DiscordApplicationCommandOptionChoice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordApplicationCommandOptionChoice"/> class.
    /// </summary>
    public DiscordApplicationCommandOptionChoice(string name, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        Name = name;
        Value = value;
    }

    /// <summary>Gets the user-facing choice name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>Gets the choice value.</summary>
    [JsonPropertyName("value")]
    public object Value { get; }
}
