namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Describes the outcome status for a gateway dispatch handler invocation.
/// </summary>
public enum GatewayHandlerExecutionStatus
{
    /// <summary>
    /// The handler completed successfully.
    /// </summary>
    Succeeded = 0,

    /// <summary>
    /// The handler intentionally skipped the dispatch.
    /// </summary>
    Skipped = 1,

    /// <summary>
    /// The handler failed in an expected or unexpected way.
    /// </summary>
    Failed = 2
}
