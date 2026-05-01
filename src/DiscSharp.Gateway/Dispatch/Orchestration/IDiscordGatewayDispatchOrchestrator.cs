namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Orchestrates all handlers that match a typed Discord gateway dispatch envelope.
/// </summary>
public interface IDiscordGatewayDispatchOrchestrator
{
    /// <summary>
    /// Dispatches the envelope to all matching handlers according to configured orchestration policy.
    /// </summary>
    ValueTask<GatewayDispatchOrchestrationResult> DispatchAsync(
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken);
}
