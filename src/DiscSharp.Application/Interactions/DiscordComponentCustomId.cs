using System.Collections.ObjectModel;
using System.Text;

namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents a compact, validated Discord component or modal custom identifier.
/// </summary>
public sealed record DiscordComponentCustomId
{
    /// <summary>
    /// Discord's maximum custom ID length for components and modals.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordComponentCustomId"/> class.
    /// </summary>
    public DiscordComponentCustomId(
        string module,
        string action,
        IReadOnlyDictionary<string, string>? arguments = null)
    {
        ValidateSegment(module, nameof(module));
        ValidateSegment(action, nameof(action));

        Module = module;
        Action = action;
        var orderedArguments = new SortedDictionary<string, string>(StringComparer.Ordinal);
        if (arguments is not null)
        {
            foreach (var pair in arguments)
            {
                orderedArguments[pair.Key] = pair.Value;
            }
        }

        Arguments = new ReadOnlyDictionary<string, string>(orderedArguments);

        var serialized = ToString();
        if (serialized.Length > MaxLength)
        {
            throw new ArgumentOutOfRangeException(nameof(arguments), serialized.Length, $"Discord custom IDs must be <= {MaxLength} characters.");
        }
    }

    /// <summary>
    /// Gets the module namespace.
    /// </summary>
    public string Module { get; }

    /// <summary>
    /// Gets the module action.
    /// </summary>
    public string Action { get; }

    /// <summary>
    /// Gets the parsed arguments.
    /// </summary>
    public IReadOnlyDictionary<string, string> Arguments { get; }

    /// <summary>
    /// Parses a serialized component custom ID.
    /// </summary>
    public static DiscordComponentCustomId Parse(string value)
    {
        if (!TryParse(value, out var customId, out var error))
        {
            throw new FormatException(error ?? "Invalid Discord component custom ID.");
        }

        return customId;
    }

    /// <summary>
    /// Attempts to parse a serialized component custom ID.
    /// </summary>
    public static bool TryParse(string? value, out DiscordComponentCustomId customId, out string? error)
    {
        customId = null!;
        error = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            error = "Custom ID is required.";
            return false;
        }

        if (value.Length > MaxLength)
        {
            error = $"Custom ID length {value.Length} exceeds Discord limit {MaxLength}.";
            return false;
        }

        var queryStart = value.IndexOf('?', StringComparison.Ordinal);
        var route = queryStart >= 0 ? value[..queryStart] : value;
        var query = queryStart >= 0 ? value[(queryStart + 1)..] : string.Empty;

        var slash = route.IndexOf('/', StringComparison.Ordinal);
        if (slash <= 0 || slash == route.Length - 1)
        {
            error = "Custom ID must use 'module/action' route syntax.";
            return false;
        }

        var module = route[..slash];
        var action = route[(slash + 1)..];

        if (!IsValidSegment(module) || !IsValidSegment(action))
        {
            error = "Custom ID module and action must contain only letters, digits, '.', '_', or '-'.";
            return false;
        }

        var arguments = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(query))
        {
            foreach (var pair in query.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var equals = pair.IndexOf('=', StringComparison.Ordinal);
                if (equals <= 0)
                {
                    error = $"Custom ID query segment '{pair}' is invalid.";
                    return false;
                }

                var key = Uri.UnescapeDataString(pair[..equals]);
                var argValue = Uri.UnescapeDataString(pair[(equals + 1)..]);
                if (!IsValidSegment(key))
                {
                    error = $"Custom ID query key '{key}' is invalid.";
                    return false;
                }

                arguments[key] = argValue;
            }
        }

        try
        {
            customId = new DiscordComponentCustomId(module, action, arguments);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentException or ArgumentOutOfRangeException)
        {
            error = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// Serializes this custom ID using deterministic key ordering.
    /// </summary>
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Module);
        builder.Append('/');
        builder.Append(Action);

        if (Arguments.Count > 0)
        {
            builder.Append('?');
            var first = true;
            foreach (var pair in Arguments.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
            {
                if (!first)
                {
                    builder.Append('&');
                }

                first = false;
                builder.Append(Uri.EscapeDataString(pair.Key));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(pair.Value));
            }
        }

        return builder.ToString();
    }

    private static void ValidateSegment(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (!IsValidSegment(value))
        {
            throw new ArgumentException("Segment must contain only letters, digits, '.', '_', or '-'.", parameterName);
        }
    }

    private static bool IsValidSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        foreach (var c in value)
        {
            if (!char.IsAsciiLetterOrDigit(c) && c is not '.' and not '_' and not '-')
            {
                return false;
            }
        }

        return true;
    }
}
