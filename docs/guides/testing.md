# Testing

DiscSharp tests should protect behavior and public payload shape. They should not only prove that a mock method was called.

## Current test projects

| Project | Purpose |
| --- | --- |
| `DiscSharp.Rest.Tests` | route/query construction, rate-limit parsing, payload serialization, REST client HTTP behavior |
| `DiscSharp.Gateway.Tests` | handler catalog and dispatch orchestration behavior |
| `DiscSharp.Application.Tests` | interaction custom ID parsing and interaction pipeline behavior |

## Commands

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## REST tests

REST tests should verify exact paths and query strings:

```csharp
[Fact]
public void CreateInteractionResponse_UsesDiscordCallbackRoute()
{
    var route = DiscordApiRoutes.CreateInteractionResponse(
        new DiscordSnowflake("123456789012345678"),
        "token-value");

    Assert.Equal(HttpMethod.Post, route.Method);
    Assert.Equal("interactions/123456789012345678/token-value/callback", route.Path);
}
```

Payload tests should serialize to JSON and assert Discord field names, callback types, flags, and component nesting.

## Gateway orchestration tests

Minimum behaviors:

- no handlers returns a successful unhandled result;
- handlers run in order;
- same order band can run in parallel mode;
- `Continue` failure policy allows later handlers;
- `StopPipeline` failure policy stops later handlers;
- timeout returns a failed handler result;
- cancellation is honored.

## Interaction pipeline tests

Minimum behaviors:

- first matching module handles the interaction;
- modules are ordered;
- unhandled fallback response is generated when enabled;
- module exception fallback response is generated when enabled;
- custom ID parser round-trips module/action/arguments;
- malformed custom IDs are rejected.

## Test doubles

Prefer simple fakes for behavior tests:

```csharp
private sealed class CapturingInteractionResponseWriter : IInteractionResponseWriter
{
    public List<InteractionResponsePlan> Responses { get; } = [];

    public ValueTask WriteAsync(
        DiscordInteractionEnvelope interaction,
        InteractionResponsePlan response,
        CancellationToken cancellationToken)
    {
        Responses.Add(response);
        return ValueTask.CompletedTask;
    }
}
```

Use NSubstitute for interaction with external dependencies where a fake would obscure intent.

## Live smoke tests

Live Discord smoke tests should remain explicit and opt-in. They require a bot token, development guild/server ID, safe test channel ID, no hard-coded tokens, and cleanup of created messages/commands.

Recommended environment variable names:

```text
DISCSHARP_DISCORD_TOKEN
DISCSHARP_TEST_GUILD_ID
DISCSHARP_TEST_CHANNEL_ID
```
