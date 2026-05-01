using DiscSharp.Rest.Primitives;

namespace DiscSharp.Rest.Routing;

/// <summary>
/// Central route factory for Discord API paths used by DiscSharp.
/// </summary>
public static class DiscordApiRoutes
{
    /// <summary>
    /// Creates the route for the interaction callback endpoint.
    /// </summary>
    public static DiscordApiRoute CreateInteractionResponse(DiscordSnowflake interactionId, string interactionToken) =>
        new(HttpMethod.Post, $"interactions/{interactionId}/{Uri.EscapeDataString(interactionToken)}/callback");

    /// <summary>
    /// Creates the route for retrieving the original interaction response.
    /// </summary>
    public static DiscordApiRoute GetOriginalInteractionResponse(DiscordSnowflake applicationId, string interactionToken) =>
        new(HttpMethod.Get, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/@original");

    /// <summary>
    /// Creates the route for editing the original interaction response.
    /// </summary>
    public static DiscordApiRoute EditOriginalInteractionResponse(DiscordSnowflake applicationId, string interactionToken) =>
        new(HttpMethod.Patch, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/@original");

    /// <summary>
    /// Creates the route for deleting the original interaction response.
    /// </summary>
    public static DiscordApiRoute DeleteOriginalInteractionResponse(DiscordSnowflake applicationId, string interactionToken) =>
        new(HttpMethod.Delete, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/@original");

    /// <summary>
    /// Creates the route for sending a followup message for an interaction.
    /// </summary>
    public static DiscordApiRoute CreateInteractionFollowup(DiscordSnowflake applicationId, string interactionToken) =>
        new(HttpMethod.Post, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}");

    /// <summary>
    /// Creates the route for retrieving an interaction followup message.
    /// </summary>
    public static DiscordApiRoute GetInteractionFollowup(DiscordSnowflake applicationId, string interactionToken, DiscordSnowflake messageId) =>
        new(HttpMethod.Get, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/{messageId}");

    /// <summary>
    /// Creates the route for editing an interaction followup message.
    /// </summary>
    public static DiscordApiRoute EditInteractionFollowup(DiscordSnowflake applicationId, string interactionToken, DiscordSnowflake messageId) =>
        new(HttpMethod.Patch, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/{messageId}");

    /// <summary>
    /// Creates the route for deleting an interaction followup message.
    /// </summary>
    public static DiscordApiRoute DeleteInteractionFollowup(DiscordSnowflake applicationId, string interactionToken, DiscordSnowflake messageId) =>
        new(HttpMethod.Delete, $"webhooks/{applicationId}/{Uri.EscapeDataString(interactionToken)}/messages/{messageId}");
}
