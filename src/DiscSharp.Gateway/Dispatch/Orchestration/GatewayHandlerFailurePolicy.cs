namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Defines how the dispatch orchestrator should react when a gateway handler fails.
/// </summary>
public enum GatewayHandlerFailurePolicy
{
    /// <summary>
    /// Records the failure and continues invoking remaining matching handlers.
    /// </summary>
    Continue = 0,

    /// <summary>
    /// Records the failure and stops invoking remaining matching handlers for the current dispatch.
    /// </summary>
    StopPipeline = 1
}
