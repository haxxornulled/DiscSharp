namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Represents the complete result of a gateway dispatch orchestration run.
/// </summary>
public sealed record GatewayDispatchOrchestrationResult(
    string EventName,
    Type PayloadType,
    long? SequenceNumber,
    int MatchedHandlerCount,
    IReadOnlyList<GatewayHandlerInvocationResult> HandlerResults,
    TimeSpan Duration)
{
    /// <summary>
    /// Gets a value indicating whether at least one handler failed.
    /// </summary>
    public bool HasFailures => HandlerResults.Any(static result => result.IsFailure);

    /// <summary>
    /// Gets a value indicating whether no handlers matched the dispatch.
    /// </summary>
    public bool WasUnhandled => MatchedHandlerCount == 0;
}
