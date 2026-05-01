namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents the result returned by an interaction module.
/// </summary>
public sealed record InteractionModuleResult
{
    private InteractionModuleResult(
        bool handled,
        InteractionResponsePlan responsePlan,
        string? message,
        Exception? exception)
    {
        Handled = handled;
        ResponsePlan = responsePlan;
        Message = message;
        Exception = exception;
    }

    /// <summary>
    /// Gets a value indicating whether this module handled the interaction.
    /// </summary>
    public bool Handled { get; }

    /// <summary>
    /// Gets the response plan.
    /// </summary>
    public InteractionResponsePlan ResponsePlan { get; }

    /// <summary>
    /// Gets the optional message.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the exception associated with a failed result.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a value indicating whether this result represents a failure.
    /// </summary>
    public bool Failed => Exception is not null;

    /// <summary>
    /// Creates a handled result.
    /// </summary>
    public static InteractionModuleResult HandledWith(InteractionResponsePlan responsePlan, string? message = null)
    {
        ArgumentNullException.ThrowIfNull(responsePlan);
        return new InteractionModuleResult(handled: true, responsePlan, message, exception: null);
    }

    /// <summary>
    /// Creates an unhandled result.
    /// </summary>
    public static InteractionModuleResult NotHandled(string? message = null) =>
        new(handled: false, InteractionResponsePlan.None(), message, exception: null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static InteractionModuleResult FailedWith(string message, Exception? exception = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new InteractionModuleResult(handled: false, InteractionResponsePlan.None(), message, exception ?? new InvalidOperationException(message));
    }
}
