# REST API

`DiscSharp.Rest` owns the current Discord HTTP API primitives. The first implemented client surface is interaction callback and followup webhook behavior because interactions are the immediate bridge between Gateway dispatch and application behavior.

## Namespaces

```csharp
using DiscSharp.Rest.DependencyInjection;
using DiscSharp.Rest.Http;
using DiscSharp.Rest.Interactions;
using DiscSharp.Rest.Primitives;
using DiscSharp.Rest.RateLimits;
using DiscSharp.Rest.Routing;
```

## Configure REST

```csharp
builder.RegisterModule(new DiscordRestModule(options =>
{
    options.BaseUri = new Uri("https://discord.com/api/", UriKind.Absolute);
    options.ApiVersion = 10;
    options.UserAgent = "DiscordBot (https://github.com/your-org/your-bot, 1.0.0)";
    options.InteractionInitialResponseDeadline = TimeSpan.FromSeconds(3);
    options.InteractionTokenLifetime = TimeSpan.FromMinutes(15);
}));
```

`DiscordApiOptions.Validate()` rejects invalid base URIs, blank user agents, API versions below 10, and broken interaction timing configuration.

## API versioning

`DiscordApiOptions.VersionedBaseUri` resolves to:

```text
https://discord.com/api/v10/
```

The route objects are intentionally relative to that versioned base URI.

## Routes

Use `DiscordApiRoutes` rather than hand-concatenating strings:

```csharp
var route = DiscordApiRoutes.CreateInteractionResponse(
    new DiscordSnowflake("123456789012345678"),
    interactionToken);

Console.WriteLine(route.Method); // POST
Console.WriteLine(route.Path);   // interactions/{id}/{token}/callback
```

Currently implemented route factories:

| Method | Factory | Discord route |
| --- | --- | --- |
| `POST` | `CreateInteractionResponse` | `/interactions/{interaction.id}/{interaction.token}/callback` |
| `GET` | `GetOriginalInteractionResponse` | `/webhooks/{application.id}/{interaction.token}/messages/@original` |
| `PATCH` | `EditOriginalInteractionResponse` | `/webhooks/{application.id}/{interaction.token}/messages/@original` |
| `DELETE` | `DeleteOriginalInteractionResponse` | `/webhooks/{application.id}/{interaction.token}/messages/@original` |
| `POST` | `CreateInteractionFollowup` | `/webhooks/{application.id}/{interaction.token}` |
| `GET` | `GetInteractionFollowup` | `/webhooks/{application.id}/{interaction.token}/messages/{message.id}` |
| `PATCH` | `EditInteractionFollowup` | `/webhooks/{application.id}/{interaction.token}/messages/{message.id}` |
| `DELETE` | `DeleteInteractionFollowup` | `/webhooks/{application.id}/{interaction.token}/messages/{message.id}` |

## Query strings

Use `DiscordQueryStringBuilder` for Discord-specific query rules:

```csharp
var query = new DiscordQueryStringBuilder()
    .Add("with_response", true)
    .AddRepeated("id", new[]
    {
        "111111111111111111",
        "222222222222222222"
    })
    .ToString();

// ?with_response=true&id=111111111111111111&id=222222222222222222
```

Discord arrays are repeated keys unless a specific endpoint documents otherwise. Do not serialize arrays as comma-separated values by default.

## Interaction responses

### Immediate ephemeral response

```csharp
await interactionClient.CreateInteractionResponseAsync(
    interactionId,
    interactionToken,
    DiscordInteractionResponsePayload.Message("Done.", ephemeral: true),
    cancellationToken: cancellationToken);
```

### Deferred response

Use this when work may exceed the initial interaction response deadline:

```csharp
await interactionClient.CreateInteractionResponseAsync(
    interactionId,
    interactionToken,
    DiscordInteractionResponsePayload.DeferChannelMessage(ephemeral: true),
    cancellationToken: cancellationToken);
```

Then edit the original response or send a followup:

```csharp
await interactionClient.EditOriginalResponseAsync(
    applicationId,
    interactionToken,
    new DiscordWebhookMessagePayload
    {
        Content = "Work complete."
    },
    cancellationToken);
```

### Modal response

```csharp
var modal = DiscordInteractionResponsePayload.Modal(
    customId: "raid:create-modal",
    title: "Create Raid",
    components:
    [
        DiscordComponent.LabelComponent(
            label: "Raid name",
            component: DiscordComponent.TextInput(
                customId: "raid:name",
                style: DiscordTextInputStyle.Short,
                required: true))
    ]);

await interactionClient.CreateInteractionResponseAsync(
    interactionId,
    interactionToken,
    modal,
    cancellationToken: cancellationToken);
```

### Autocomplete response

```csharp
var response = DiscordInteractionResponsePayload.Autocomplete(
[
    new DiscordApplicationCommandOptionChoice("Heroic", "heroic"),
    new DiscordApplicationCommandOptionChoice("Mythic", "mythic")
]);
```

Autocomplete is capped at 25 choices by the payload factory.

## Followups

```csharp
var message = await interactionClient.CreateFollowupMessageAsync(
    applicationId,
    interactionToken,
    new DiscordWebhookMessagePayload
    {
        Content = "Additional details go here."
    },
    cancellationToken);
```

Followups use the interaction token and webhook-style endpoints. Treat the token as short-lived and sensitive.

## Rate-limit headers

`DiscordRateLimitHeaders` parses rate-limit metadata from HTTP response headers. This exists so future REST clients can be bucket-aware instead of sleeping blindly.

```csharp
var parsed = DiscordRateLimitHeaders.From(response.Headers, response.Content.Headers);

if (parsed.RetryAfter is { } retryAfter)
{
    logger.LogWarning("Discord rate limit retry after {RetryAfter}.", retryAfter);
}
```

Track bucket, limit, remaining, reset, reset-after, retry-after, global, and scope in logs/telemetry where available.

## Error handling

`DiscordRestException` carries the HTTP status, Discord error body, and route context. Do not collapse this to a string in production logs.

```csharp
catch (DiscordRestException ex)
{
    logger.LogError(
        ex,
        "Discord REST request failed. StatusCode={StatusCode} DiscordCode={DiscordCode} Route={Route}",
        ex.StatusCode,
        ex.Error?.Code,
        ex.Route);

    throw;
}
```

## Current limitations

This pass does not yet implement the full Discord REST surface. The current REST project is the foundation for route/query construction, rate-limit parsing, Discord error handling, interaction callback/followup endpoints, and component/modal payload serialization.
