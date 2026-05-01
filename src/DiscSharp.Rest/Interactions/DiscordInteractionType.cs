namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Discord interaction type values.
/// </summary>
public enum DiscordInteractionType
{
    /// <summary>Discord ping validation interaction.</summary>
    Ping = 1,

    /// <summary>Application command interaction.</summary>
    ApplicationCommand = 2,

    /// <summary>Message component interaction.</summary>
    MessageComponent = 3,

    /// <summary>Application command autocomplete interaction.</summary>
    ApplicationCommandAutocomplete = 4,

    /// <summary>Modal submit interaction.</summary>
    ModalSubmit = 5
}
