namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Represents a single handler invocation result inside a gateway dispatch orchestration run.
/// </summary>
public sealed record GatewayHandlerInvocationResult(
    string HandlerName,
    string EventName,
    Type PayloadType,
    int Order,
    GatewayHandlerExecutionStatus Status,
    TimeSpan Duration,
    string? Message,
    Exception? Exception)
{
    /// <summary>
    /// Gets a value indicating whether the handler invocation failed.
    /// </summary>
    public bool IsFailure => Status == GatewayHandlerExecutionStatus.Failed;
}
