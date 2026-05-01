namespace DiscSharp.Application.Interactions;

/// <summary>
/// Writes application-level interaction response plans to Discord through the REST layer.
/// </summary>
public interface IInteractionResponseWriter
{
    /// <summary>
    /// Writes the supplied response plan for an interaction.
    /// </summary>
    ValueTask WriteAsync(
        DiscordInteractionEnvelope interaction,
        InteractionResponsePlan responsePlan,
        CancellationToken cancellationToken);
}
