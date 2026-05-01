namespace DiscSharp.Rest.Primitives;

/// <summary>
/// Represents a Discord snowflake identifier while preserving Discord's string JSON contract.
/// </summary>
public readonly record struct DiscordSnowflake
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordSnowflake"/> struct.
    /// </summary>
    public DiscordSnowflake(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (!ulong.TryParse(value, out _))
        {
            throw new ArgumentException("Discord snowflakes must be unsigned 64-bit integer strings.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Gets the snowflake string value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a snowflake from an unsigned integer value.
    /// </summary>
    public static DiscordSnowflake FromUInt64(ulong value) => new(value.ToString(System.Globalization.CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Converts a snowflake to its string representation.
    /// </summary>
    public static implicit operator string(DiscordSnowflake snowflake) => snowflake.Value;

    /// <summary>
    /// Converts a string to a Discord snowflake.
    /// </summary>
    public static explicit operator DiscordSnowflake(string value) => new(value);
}
