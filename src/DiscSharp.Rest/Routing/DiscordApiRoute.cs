using DiscSharp.Rest.Http;

namespace DiscSharp.Rest.Routing;

/// <summary>
/// Represents a Discord API route relative to the versioned API base URI.
/// </summary>
public sealed record DiscordApiRoute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordApiRoute"/> class.
    /// </summary>
    public DiscordApiRoute(HttpMethod method, string path)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Method = method;
        Path = path.TrimStart('/');
    }

    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public HttpMethod Method { get; }

    /// <summary>
    /// Gets the path relative to <c>/api/v{version}</c>.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// Builds an absolute request URI using the supplied options and optional query string.
    /// </summary>
    public Uri BuildUri(DiscordApiOptions options, string? queryString = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        var relative = string.IsNullOrWhiteSpace(queryString)
            ? Path
            : $"{Path}?{queryString.TrimStart('?')}";

        return new Uri(options.VersionedBaseUri, relative);
    }

    /// <summary>
    /// Returns the HTTP method and relative path in a human-readable route format.
    /// </summary>
    public override string ToString() => $"{Method} /{Path}";
}
