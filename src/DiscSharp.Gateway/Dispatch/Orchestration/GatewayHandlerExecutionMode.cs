namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Defines how matching gateway dispatch handlers are invoked.
/// </summary>
public enum GatewayHandlerExecutionMode
{
    /// <summary>
    /// Executes handlers one at a time in ascending order.
    /// </summary>
    Sequential = 0,

    /// <summary>
    /// Executes each order band sequentially, while handlers inside the same order band run concurrently.
    /// </summary>
    ParallelByOrderBand = 1
}
