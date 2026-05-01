namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Configures gateway dispatch orchestration behavior.
/// </summary>
public sealed class GatewayDispatchOrchestrationOptions
{
    /// <summary>
    /// Gets or sets the configuration section name.
    /// </summary>
    public const string SectionName = "DiscSharp:Gateway:DispatchOrchestration";

    /// <summary>
    /// Gets or sets the handler execution mode.
    /// </summary>
    public GatewayHandlerExecutionMode ExecutionMode { get; set; } = GatewayHandlerExecutionMode.Sequential;

    /// <summary>
    /// Gets or sets the per-handler timeout. A null value disables local handler timeout wrapping.
    /// </summary>
    public TimeSpan? HandlerTimeout { get; set; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Gets or sets whether an event with no matching handlers should be logged at debug level.
    /// </summary>
    public bool LogUnhandledDispatches { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of handlers allowed in one parallel order band.
    /// </summary>
    public int MaxParallelHandlersPerOrderBand { get; set; } = 16;

    /// <summary>
    /// Validates this options instance.
    /// </summary>
    public void Validate()
    {
        if (HandlerTimeout is { } timeout && timeout <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Gateway handler timeout must be greater than zero when configured.");
        }

        if (MaxParallelHandlersPerOrderBand <= 0)
        {
            throw new InvalidOperationException("MaxParallelHandlersPerOrderBand must be greater than zero.");
        }
    }
}
