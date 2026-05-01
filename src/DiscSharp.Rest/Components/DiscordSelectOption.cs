using System.Text.Json.Serialization;

namespace DiscSharp.Rest.Components;

/// <summary>
/// Represents an option in a Discord string select menu or radio group.
/// </summary>
public sealed record DiscordSelectOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordSelectOption"/> class.
    /// </summary>
    public DiscordSelectOption(string label, string value, string? description = null, bool? isDefault = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (label.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(label), label.Length, "Discord option labels must be <= 100 characters.");
        }

        if (value.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value.Length, "Discord option values must be <= 100 characters.");
        }

        if (description is { Length: > 100 })
        {
            throw new ArgumentOutOfRangeException(nameof(description), description.Length, "Discord option descriptions must be <= 100 characters.");
        }

        Label = label;
        Value = value;
        Description = description;
        Default = isDefault;
    }

    /// <summary>Gets the user-facing option label.</summary>
    [JsonPropertyName("label")]
    public string Label { get; }

    /// <summary>Gets the developer-defined option value.</summary>
    [JsonPropertyName("value")]
    public string Value { get; }

    /// <summary>Gets the optional option description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; }

    /// <summary>Gets whether the option is selected by default.</summary>
    [JsonPropertyName("default")]
    public bool? Default { get; }
}
