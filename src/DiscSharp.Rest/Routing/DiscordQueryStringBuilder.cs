using System.Globalization;
using System.Text;
using DiscSharp.Rest.Primitives;

namespace DiscSharp.Rest.Routing;

/// <summary>
/// Builds Discord-compatible query strings, including repeated-key array parameters.
/// </summary>
public sealed class DiscordQueryStringBuilder
{
    private readonly List<KeyValuePair<string, string>> _values = [];

    /// <summary>
    /// Adds a string query parameter when the value is not null or whitespace.
    /// </summary>
    public DiscordQueryStringBuilder Add(string name, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (!string.IsNullOrWhiteSpace(value))
        {
            _values.Add(new KeyValuePair<string, string>(name, value));
        }

        return this;
    }

    /// <summary>
    /// Adds a Boolean query parameter using Discord's lower-case true/false representation.
    /// </summary>
    public DiscordQueryStringBuilder AddBoolean(string name, bool? value)
    {
        if (value.HasValue)
        {
            Add(name, value.Value ? "true" : "false");
        }

        return this;
    }

    /// <summary>
    /// Adds an integer query parameter.
    /// </summary>
    public DiscordQueryStringBuilder AddInteger(string name, int? value)
    {
        if (value.HasValue)
        {
            Add(name, value.Value.ToString(CultureInfo.InvariantCulture));
        }

        return this;
    }

    /// <summary>
    /// Adds an array of snowflakes using Discord's repeated-key query format.
    /// </summary>
    public DiscordQueryStringBuilder AddSnowflakeArray(string name, IEnumerable<DiscordSnowflake>? values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (values is null)
        {
            return this;
        }

        foreach (var value in values)
        {
            Add(name, value.Value);
        }

        return this;
    }

    /// <summary>
    /// Builds the query string without a leading question mark.
    /// </summary>
    public string Build()
    {
        if (_values.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < _values.Count; i++)
        {
            if (i > 0)
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(_values[i].Key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(_values[i].Value));
        }

        return builder.ToString();
    }
}
