namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Finds registered handlers for gateway dispatch envelopes.
/// </summary>
public interface IGatewayDispatchHandlerCatalog
{
    /// <summary>
    /// Finds matching handlers for the supplied gateway dispatch envelope.
    /// </summary>
    IReadOnlyList<IDiscordGatewayDispatchHandler> FindHandlers(GatewayDispatchEnvelope envelope);

    /// <summary>
    /// Lists all registered handlers for diagnostics.
    /// </summary>
    IReadOnlyList<GatewayDispatchHandlerDescriptor> DescribeHandlers();
}
