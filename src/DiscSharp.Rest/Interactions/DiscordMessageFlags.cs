namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Message flags supported by Discord interaction callback data.
/// </summary>
[Flags]
public enum DiscordMessageFlags
{
    /// <summary>No flags.</summary>
    None = 0,

    /// <summary>Suppress embeds.</summary>
    SuppressEmbeds = 1 << 2,

    /// <summary>Only the invoking user can see the message.</summary>
    Ephemeral = 1 << 6,

    /// <summary>Suppress push and desktop notifications.</summary>
    SuppressNotifications = 1 << 12,

    /// <summary>Use Discord Components V2 rendering rules.</summary>
    IsComponentsV2 = 1 << 15,

    /// <summary>Voice message flag.</summary>
    IsVoiceMessage = 1 << 13
}
