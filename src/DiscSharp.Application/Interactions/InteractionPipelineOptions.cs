namespace DiscSharp.Application.Interactions;

/// <summary>
/// Configures the Discord interaction pipeline.
/// </summary>
public sealed class InteractionPipelineOptions
{
    /// <summary>
    /// Gets the configuration section name.
    /// </summary>
    public const string SectionName = "DiscSharp:Interactions:Pipeline";

    /// <summary>
    /// Gets or sets whether an unhandled interaction should receive a safe ephemeral response.
    /// </summary>
    public bool RespondToUnhandledInteractions { get; set; } = true;

    /// <summary>
    /// Gets or sets whether module exceptions should receive a safe ephemeral failure response.
    /// </summary>
    public bool RespondToModuleFailures { get; set; } = true;

    /// <summary>
    /// Gets or sets the unhandled response content.
    /// </summary>
    public string UnhandledResponseContent { get; set; } = "That interaction is not handled by this application module.";

    /// <summary>
    /// Gets or sets the module failure response content.
    /// </summary>
    public string FailureResponseContent { get; set; } = "Something went wrong while handling that interaction.";
}
