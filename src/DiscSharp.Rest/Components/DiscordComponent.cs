using System.Text.Json.Serialization;
using DiscSharp.Rest.Primitives;

namespace DiscSharp.Rest.Components;

/// <summary>
/// Represents a Discord component payload. This intentionally models the superset shape so DiscSharp can track Discord's expanding component surface without serializer hacks.
/// </summary>
public sealed record DiscordComponent
{
    private DiscordComponent(DiscordComponentType type)
    {
        Type = type;
    }

    /// <summary>Gets the component type.</summary>
    [JsonPropertyName("type")]
    public DiscordComponentType Type { get; init; }

    /// <summary>Gets the optional Discord component identifier.</summary>
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    /// <summary>Gets child components for action rows, sections, or containers.</summary>
    [JsonPropertyName("components")]
    public IReadOnlyList<DiscordComponent>? Components { get; init; }

    /// <summary>Gets the single child component for labels.</summary>
    [JsonPropertyName("component")]
    public DiscordComponent? Component { get; init; }

    /// <summary>Gets the component custom ID.</summary>
    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }

    /// <summary>Gets the button or text-input style.</summary>
    [JsonPropertyName("style")]
    public int? Style { get; init; }

    /// <summary>Gets the user-facing label.</summary>
    [JsonPropertyName("label")]
    public string? Label { get; init; }

    /// <summary>Gets the component description.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>Gets text-display markdown content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>Gets the link button URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>Gets the premium SKU ID.</summary>
    [JsonPropertyName("sku_id")]
    public string? SkuId { get; init; }

    /// <summary>Gets whether the component is disabled.</summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    /// <summary>Gets select or radio options.</summary>
    [JsonPropertyName("options")]
    public IReadOnlyList<DiscordSelectOption>? Options { get; init; }

    /// <summary>Gets the placeholder text.</summary>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; init; }

    /// <summary>Gets the minimum selectable/uploadable values or text length.</summary>
    [JsonPropertyName("min_values")]
    public int? MinValues { get; init; }

    /// <summary>Gets the maximum selectable/uploadable values or text length.</summary>
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; init; }

    /// <summary>Gets the minimum text length.</summary>
    [JsonPropertyName("min_length")]
    public int? MinLength { get; init; }

    /// <summary>Gets the maximum text length.</summary>
    [JsonPropertyName("max_length")]
    public int? MaxLength { get; init; }

    /// <summary>Gets whether the modal component is required.</summary>
    [JsonPropertyName("required")]
    public bool? Required { get; init; }

    /// <summary>Gets a pre-filled value.</summary>
    [JsonPropertyName("value")]
    public string? Value { get; init; }

    /// <summary>Gets channel type filters for channel selects.</summary>
    [JsonPropertyName("channel_types")]
    public IReadOnlyList<int>? ChannelTypes { get; init; }

    /// <summary>Creates an action row component.</summary>
    public static DiscordComponent ActionRow(params DiscordComponent[] components)
    {
        ArgumentNullException.ThrowIfNull(components);
        if (components.Length is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(components), components.Length, "Action rows must contain between 1 and 5 child components.");
        }

        return new DiscordComponent(DiscordComponentType.ActionRow) { Components = components };
    }

    /// <summary>Creates a non-link button component.</summary>
    public static DiscordComponent Button(string customId, string label, DiscordButtonStyle style = DiscordButtonStyle.Primary, bool disabled = false)
    {
        ValidateCustomId(customId);
        ValidateLength(label, nameof(label), 80);
        if (style is DiscordButtonStyle.Link or DiscordButtonStyle.Premium)
        {
            throw new ArgumentException("Use LinkButton or PremiumButton for link and premium styles.", nameof(style));
        }

        return new DiscordComponent(DiscordComponentType.Button)
        {
            CustomId = customId,
            Label = label,
            Style = (int)style,
            Disabled = disabled ? true : null
        };
    }

    /// <summary>Creates a link button component.</summary>
    public static DiscordComponent LinkButton(string url, string label, bool disabled = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ValidateLength(label, nameof(label), 80);
        return new DiscordComponent(DiscordComponentType.Button)
        {
            Url = url,
            Label = label,
            Style = (int)DiscordButtonStyle.Link,
            Disabled = disabled ? true : null
        };
    }

    /// <summary>Creates a premium button component.</summary>
    public static DiscordComponent PremiumButton(DiscordSnowflake skuId, bool disabled = false) =>
        new(DiscordComponentType.Button)
        {
            SkuId = skuId.Value,
            Style = (int)DiscordButtonStyle.Premium,
            Disabled = disabled ? true : null
        };

    /// <summary>Creates a string select menu component.</summary>
    public static DiscordComponent StringSelect(string customId, IReadOnlyList<DiscordSelectOption> options, string? placeholder = null, int? minValues = null, int? maxValues = null, bool? required = null)
    {
        ValidateCustomId(customId);
        ArgumentNullException.ThrowIfNull(options);
        if (options.Count is < 1 or > 25)
        {
            throw new ArgumentOutOfRangeException(nameof(options), options.Count, "String select menus must contain between 1 and 25 options.");
        }

        ValidateLength(placeholder, nameof(placeholder), 150);
        ValidateRange(minValues, nameof(minValues), 0, 25);
        ValidateRange(maxValues, nameof(maxValues), 1, 25);

        return new DiscordComponent(DiscordComponentType.StringSelect)
        {
            CustomId = customId,
            Options = options,
            Placeholder = placeholder,
            MinValues = minValues,
            MaxValues = maxValues,
            Required = required
        };
    }

    /// <summary>Creates an auto-populated select menu component.</summary>
    public static DiscordComponent EntitySelect(DiscordComponentType type, string customId, string? placeholder = null, int? minValues = null, int? maxValues = null, bool? required = null, IReadOnlyList<int>? channelTypes = null)
    {
        if (type is not (DiscordComponentType.UserSelect or DiscordComponentType.RoleSelect or DiscordComponentType.MentionableSelect or DiscordComponentType.ChannelSelect))
        {
            throw new ArgumentException("Entity selects must be user, role, mentionable, or channel selects.", nameof(type));
        }

        ValidateCustomId(customId);
        ValidateLength(placeholder, nameof(placeholder), 150);
        ValidateRange(minValues, nameof(minValues), 0, 25);
        ValidateRange(maxValues, nameof(maxValues), 1, 25);

        return new DiscordComponent(type)
        {
            CustomId = customId,
            Placeholder = placeholder,
            MinValues = minValues,
            MaxValues = maxValues,
            Required = required,
            ChannelTypes = channelTypes
        };
    }

    /// <summary>Creates a text input component for modals.</summary>
    public static DiscordComponent TextInput(string customId, DiscordTextInputStyle style, int? minLength = null, int? maxLength = null, bool? required = null, string? value = null, string? placeholder = null)
    {
        ValidateCustomId(customId);
        ValidateRange(minLength, nameof(minLength), 0, 4000);
        ValidateRange(maxLength, nameof(maxLength), 1, 4000);
        ValidateLength(value, nameof(value), 4000);
        ValidateLength(placeholder, nameof(placeholder), 100);

        return new DiscordComponent(DiscordComponentType.TextInput)
        {
            CustomId = customId,
            Style = (int)style,
            MinLength = minLength,
            MaxLength = maxLength,
            Required = required,
            Value = value,
            Placeholder = placeholder
        };
    }

    /// <summary>Creates a text-display component for Components V2 messages or modals.</summary>
    public static DiscordComponent TextDisplay(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new DiscordComponent(DiscordComponentType.TextDisplay) { Content = content };
    }

    /// <summary>Creates a modal label wrapper component.</summary>
    public static DiscordComponent LabelComponent(string label, DiscordComponent component, string? description = null)
    {
        ValidateLength(label, nameof(label), 45);
        ValidateLength(description, nameof(description), 100);
        ArgumentNullException.ThrowIfNull(component);

        return new DiscordComponent(DiscordComponentType.Label)
        {
            Label = label,
            Description = description,
            Component = component
        };
    }

    /// <summary>Creates a modal file-upload component.</summary>
    public static DiscordComponent FileUpload(string customId, int? minValues = null, int? maxValues = null, bool? required = null)
    {
        ValidateCustomId(customId);
        ValidateRange(minValues, nameof(minValues), 0, 10);
        ValidateRange(maxValues, nameof(maxValues), 1, 10);
        return new DiscordComponent(DiscordComponentType.FileUpload)
        {
            CustomId = customId,
            MinValues = minValues,
            MaxValues = maxValues,
            Required = required
        };
    }

    private static void ValidateCustomId(string customId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customId);
        ValidateLength(customId, nameof(customId), 100);
    }

    private static void ValidateLength(string? value, string parameterName, int maxLength)
    {
        if (value is { Length: var length } && length > maxLength)
        {
            throw new ArgumentOutOfRangeException(parameterName, length, $"Discord field must be <= {maxLength} characters.");
        }
    }

    private static void ValidateRange(int? value, string parameterName, int min, int max)
    {
        if (value is null)
        {
            return;
        }

        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, $"Discord field must be between {min} and {max}.");
        }
    }
}
