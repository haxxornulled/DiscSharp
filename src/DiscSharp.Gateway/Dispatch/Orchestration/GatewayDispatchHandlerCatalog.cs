using Microsoft.Extensions.Logging;

namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Default in-memory handler catalog backed by DI-provided gateway dispatch handlers.
/// </summary>
public sealed class GatewayDispatchHandlerCatalog : IGatewayDispatchHandlerCatalog
{
    private readonly IReadOnlyList<IDiscordGatewayDispatchHandler> _handlers;
    private readonly ILogger<GatewayDispatchHandlerCatalog> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayDispatchHandlerCatalog"/> class.
    /// </summary>
    public GatewayDispatchHandlerCatalog(
        IEnumerable<IDiscordGatewayDispatchHandler> handlers,
        ILogger<GatewayDispatchHandlerCatalog> logger)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        ArgumentNullException.ThrowIfNull(logger);

        _handlers = handlers
            .OrderBy(static h => h.EventName, StringComparer.Ordinal)
            .ThenBy(static h => h.Order)
            .ThenBy(static h => h.HandlerName, StringComparer.Ordinal)
            .ToArray();
        _logger = logger;

        _logger.LogInformation("Gateway dispatch handler catalog initialized with {HandlerCount} handlers", _handlers.Count);
    }

    /// <inheritdoc />
    public IReadOnlyList<IDiscordGatewayDispatchHandler> FindHandlers(GatewayDispatchEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var matches = _handlers
            .Where(handler => handler.CanHandle(envelope))
            .OrderBy(static handler => handler.Order)
            .ThenBy(static handler => handler.HandlerName, StringComparer.Ordinal)
            .ToArray();

        _logger.LogDebug(
            "Resolved {HandlerCount} gateway handlers for {EventName} payload {PayloadType}",
            matches.Length,
            envelope.EventName,
            envelope.PayloadType.FullName);

        return matches;
    }

    /// <inheritdoc />
    public IReadOnlyList<GatewayDispatchHandlerDescriptor> DescribeHandlers() =>
        _handlers.Select(static h => h.Describe()).ToArray();
}
