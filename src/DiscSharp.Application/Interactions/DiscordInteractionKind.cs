namespace DiscSharp.Application.Interactions;

/// <summary>
/// Represents the high-level Discord interaction kind.
/// </summary>
public enum DiscordInteractionKind
{
    /// <summary>
    /// The interaction kind is unknown or not mapped.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Slash command or context menu command interaction.
    /// </summary>
    ApplicationCommand = 1,

    /// <summary>
    /// Message component interaction, such as a button or select menu.
    /// </summary>
    MessageComponent = 2,

    /// <summary>
    /// Autocomplete interaction.
    /// </summary>
    ApplicationCommandAutocomplete = 3,

    /// <summary>
    /// Modal submit interaction.
    /// </summary>
    ModalSubmit = 4
}
