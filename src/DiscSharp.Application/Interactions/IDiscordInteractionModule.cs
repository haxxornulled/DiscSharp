namespace DiscSharp.Application.Interactions;

/// <summary>
/// Handles Discord interactions for a specific application feature module.
/// </summary>
public interface IDiscordInteractionModule
{
    /// <summary>
    /// Gets a stable module name used for diagnostics and custom-id routing.
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// Gets the module order. Lower values run first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Determines whether this module can handle the supplied interaction.
    /// </summary>
    bool CanHandle(DiscordInteractionEnvelope interaction);

    /// <summary>
    /// Handles the supplied interaction.
    /// </summary>
    ValueTask<InteractionModuleResult> HandleAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken);
}
