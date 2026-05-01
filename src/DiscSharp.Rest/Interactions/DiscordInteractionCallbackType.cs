namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Discord interaction callback type values.
/// </summary>
public enum DiscordInteractionCallbackType
{
    /// <summary>ACK a Ping interaction.</summary>
    Pong = 1,

    /// <summary>Respond to an interaction with a message.</summary>
    ChannelMessageWithSource = 4,

    /// <summary>ACK an interaction and edit a response later.</summary>
    DeferredChannelMessageWithSource = 5,

    /// <summary>ACK a component interaction and edit the source message later.</summary>
    DeferredUpdateMessage = 6,

    /// <summary>Edit the message the component was attached to.</summary>
    UpdateMessage = 7,

    /// <summary>Respond to an autocomplete interaction.</summary>
    ApplicationCommandAutocompleteResult = 8,

    /// <summary>Respond to an interaction with a modal popup.</summary>
    Modal = 9,

    /// <summary>Launch the associated Activity.</summary>
    LaunchActivity = 12
}
