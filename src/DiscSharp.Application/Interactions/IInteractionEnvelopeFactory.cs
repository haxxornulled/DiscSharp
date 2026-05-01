namespace DiscSharp.Application.Interactions;

/// <summary>
/// Creates an application-facing interaction envelope from an existing typed gateway interaction payload.
/// </summary>
/// <typeparam name="TInteractionCreateEvent">The existing typed gateway interaction event DTO.</typeparam>
public interface IInteractionEnvelopeFactory<in TInteractionCreateEvent>
    where TInteractionCreateEvent : class
{
    /// <summary>
    /// Creates a Discord interaction envelope.
    /// </summary>
    DiscordInteractionEnvelope Create(TInteractionCreateEvent interactionCreateEvent, DateTimeOffset receivedAt);
}
