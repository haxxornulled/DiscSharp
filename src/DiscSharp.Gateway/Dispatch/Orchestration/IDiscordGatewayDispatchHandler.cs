namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Handles a typed Discord gateway dispatch envelope.
/// </summary>
public interface IDiscordGatewayDispatchHandler
{
    /// <summary>
    /// Gets a stable handler name used for logs, metrics, and diagnostics.
    /// </summary>
    string HandlerName { get; }

    /// <summary>
    /// Gets the Discord gateway dispatch event name handled by this handler.
    /// </summary>
    string EventName { get; }

    /// <summary>
    /// Gets the payload type handled by this handler.
    /// </summary>
    Type PayloadType { get; }

    /// <summary>
    /// Gets the handler execution order. Lower values run first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Gets the failure policy for this handler.
    /// </summary>
    GatewayHandlerFailurePolicy FailurePolicy { get; }

    /// <summary>
    /// Determines whether this handler can process the supplied dispatch envelope.
    /// </summary>
    bool CanHandle(GatewayDispatchEnvelope envelope);

    /// <summary>
    /// Handles the supplied dispatch envelope.
    /// </summary>
    ValueTask<GatewayHandlerExecutionResult> HandleAsync(
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken);

    /// <summary>
    /// Describes the handler for diagnostics.
    /// </summary>
    GatewayDispatchHandlerDescriptor Describe() =>
        new(HandlerName, EventName, PayloadType, Order, FailurePolicy);
}
