namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents the result of an interaction pipeline execution.
/// </summary>
public sealed record DiscordInteractionPipelineResult(
    string InteractionId,
    DiscordInteractionKind Kind,
    bool Handled,
    string? ModuleName,
    InteractionResponsePlan ResponsePlan,
    TimeSpan Duration,
    string? Message,
    Exception? Exception)
{
    /// <summary>
    /// Gets a value indicating whether the pipeline failed.
    /// </summary>
    public bool Failed => Exception is not null;
}
