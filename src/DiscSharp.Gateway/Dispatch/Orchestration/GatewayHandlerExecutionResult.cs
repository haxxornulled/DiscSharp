namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Represents the result of a gateway dispatch handler invocation.
/// </summary>
public sealed record GatewayHandlerExecutionResult
{
    private GatewayHandlerExecutionResult(
        GatewayHandlerExecutionStatus status,
        string? message,
        Exception? exception)
    {
        Status = status;
        Message = message;
        Exception = exception;
    }

    /// <summary>
    /// Gets the handler execution status.
    /// </summary>
    public GatewayHandlerExecutionStatus Status { get; }

    /// <summary>
    /// Gets the optional result message.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the exception that caused the failure, when available.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static GatewayHandlerExecutionResult Succeeded(string? message = null) =>
        new(GatewayHandlerExecutionStatus.Succeeded, message, exception: null);

    /// <summary>
    /// Creates a skipped result.
    /// </summary>
    public static GatewayHandlerExecutionResult Skipped(string? message = null) =>
        new(GatewayHandlerExecutionStatus.Skipped, message, exception: null);

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static GatewayHandlerExecutionResult Failed(string message, Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new GatewayHandlerExecutionResult(GatewayHandlerExecutionStatus.Failed, message, exception);
    }
}
