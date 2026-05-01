using DiscSharp.Rest.Primitives;

namespace DiscSharp.Rest.Interactions;

/// <summary>
/// Provides typed operations for Discord interaction callback and followup webhook endpoints.
/// </summary>
public interface IDiscordInteractionRestClient
{
    /// <summary>
    /// Creates the initial response for an interaction.
    /// </summary>
    ValueTask<DiscordInteractionCallbackResponse?> CreateInteractionResponseAsync(
        DiscordSnowflake interactionId,
        string interactionToken,
        DiscordInteractionResponsePayload payload,
        bool withResponse = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the original interaction response message.
    /// </summary>
    ValueTask<DiscordWebhookMessage> GetOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Edits the original interaction response message.
    /// </summary>
    ValueTask<DiscordWebhookMessage> EditOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the original interaction response message.
    /// </summary>
    ValueTask DeleteOriginalResponseAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a followup message for an interaction.
    /// </summary>
    ValueTask<DiscordWebhookMessage> CreateFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a followup message for an interaction.
    /// </summary>
    ValueTask<DiscordWebhookMessage> GetFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Edits a followup message for an interaction.
    /// </summary>
    ValueTask<DiscordWebhookMessage> EditFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        DiscordWebhookMessagePayload payload,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a followup message for an interaction.
    /// </summary>
    ValueTask DeleteFollowupMessageAsync(
        DiscordSnowflake applicationId,
        string interactionToken,
        DiscordSnowflake messageId,
        CancellationToken cancellationToken = default);
}
