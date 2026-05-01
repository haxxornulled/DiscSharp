namespace DiscSharp.Application.Interactions;

/// <summary>
/// Orchestrates Discord interaction modules.
/// </summary>
public interface IDiscordInteractionPipeline
{
    /// <summary>
    /// Executes the interaction pipeline for the supplied interaction.
    /// </summary>
    ValueTask<DiscordInteractionPipelineResult> ExecuteAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken);
}
