using System.Text.Json.Serialization;
using DiscSharp.Rest.Components;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Represents the optional data object in a Discord interaction callback response.
/// </summary>
public sealed record DiscordInteractionCallbackData
{
    /// <summary>Gets whether the response is TTS.</summary>
    [JsonPropertyName("tts")]
    public bool? Tts { get; init; }

    /// <summary>Gets message content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>Gets message flags.</summary>
    [JsonPropertyName("flags")]
    public int? Flags { get; init; }

    /// <summary>Gets allowed mentions.</summary>
    [JsonPropertyName("allowed_mentions")]
    public DiscordAllowedMentions? AllowedMentions { get; init; }

    /// <summary>Gets message or modal components.</summary>
    [JsonPropertyName("components")]
    public IReadOnlyList<DiscordComponent>? Components { get; init; }

    /// <summary>Gets autocomplete choices.</summary>
    [JsonPropertyName("choices")]
    public IReadOnlyList<DiscordApplicationCommandOptionChoice>? Choices { get; init; }

    /// <summary>Gets modal custom ID.</summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }

    /// <summary>Gets modal title.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>Creates message callback data.</summary>
    public static DiscordInteractionCallbackData Message(
        string content,
        DiscordMessageFlags flags = DiscordMessageFlags.None,
        IReadOnlyList<DiscordComponent>? components = null,
        DiscordAllowedMentions? allowedMentions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new DiscordInteractionCallbackData
        {
            Content = content,
            Flags = flags == DiscordMessageFlags.None ? null : (int)flags,
            Components = components,
            AllowedMentions = allowedMentions ?? DiscordAllowedMentions.None()
        };
    }

    /// <summary>Creates modal callback data.</summary>
    public static DiscordInteractionCallbackData Modal(string customId, string title, IReadOnlyList<DiscordComponent> components)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(components);

        if (customId.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(customId), customId.Length, "Modal custom IDs must be <= 100 characters.");
        }

        if (title.Length > 45)
        {
            throw new ArgumentOutOfRangeException(nameof(title), title.Length, "Modal titles must be <= 45 characters.");
        }

        if (components.Count is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(components), components.Count, "Modals must include between 1 and 5 top-level components.");
        }

        return new DiscordInteractionCallbackData
        {
            CustomId = customId,
            Title = title,
            Components = components
        };
    }

    /// <summary>Creates autocomplete callback data.</summary>
    public static DiscordInteractionCallbackData Autocomplete(IReadOnlyList<DiscordApplicationCommandOptionChoice> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);
        if (choices.Count > 25)
        {
            throw new ArgumentOutOfRangeException(nameof(choices), choices.Count, "Autocomplete responses support at most 25 choices.");
        }

        return new DiscordInteractionCallbackData { Choices = choices };
    }
}
