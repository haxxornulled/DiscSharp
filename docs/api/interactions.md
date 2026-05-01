# Interactions Pipeline

`DiscSharp.Application` provides a transport-neutral interaction pipeline. It exists so slash commands, buttons, selects, modal submits, autocomplete, and future app scenarios can be handled as application behavior instead of raw Discord DTO manipulation.

## Core types

| Type | Purpose |
| --- | --- |
| `DiscordInteractionEnvelope` | Application-facing representation of an interaction. |
| `DiscordInteractionKind` | High-level interaction category. |
| `IDiscordInteractionModule` | Feature module that can handle an interaction. |
| `IDiscordInteractionPipeline` | Ordered module execution pipeline. |
| `InteractionModuleResult` | Module output: handled/unhandled/response/failure. |
| `InteractionResponsePlan` | Transport-neutral plan for what should be sent back. |
| `IInteractionResponseWriter` | Port that writes response plans back to Discord. |

## Why this exists

Discord interactions can arrive over Gateway or over an HTTP interactions endpoint. In both cases, application behavior should look the same.

A Raid Manager module should not care whether the interaction was delivered by WebSocket dispatch or webhook request. It should receive an envelope, make an application decision, and return a response plan.

## Module shape

```csharp
public sealed class RaidCreateInteractionModule : IDiscordInteractionModule
{
    private readonly IRaidManagerInteractionService _raids;

    public RaidCreateInteractionModule(IRaidManagerInteractionService raids)
    {
        ArgumentNullException.ThrowIfNull(raids);
        _raids = raids;
    }

    public string ModuleName => "raid";
    public int Order => 0;

    public bool CanHandle(DiscordInteractionEnvelope interaction) =>
        interaction.Kind is DiscordInteractionKind.MessageComponent &&
        interaction.CustomId is { Module: "raid", Action: "create" };

    public async ValueTask<InteractionModuleResult> HandleAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken)
    {
        var result = await _raids.StartCreateRaidAsync(interaction.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFail)
        {
            return InteractionModuleResult.Respond(
                InteractionResponsePlan.Message("Could not start raid creation.", ephemeral: true));
        }

        return InteractionModuleResult.Respond(
            InteractionResponsePlan.Modal("raid:create", "Create Raid", result.Value.Components));
    }
}
```

## Custom IDs

Use `DiscordComponentCustomId` for deterministic component routing. Do not invent ad hoc split rules in every module.

Recommended shape:

```text
module:action;key=value;key=value
```

Example:

```csharp
var customId = DiscordComponentCustomId.Create(
    module: "raid",
    action: "join",
    arguments: new Dictionary<string, string>
    {
        ["raidId"] = raidId.ToString(CultureInfo.InvariantCulture)
    });

var button = DiscordComponent.Button(
    customId: customId.ToString(),
    label: "Join Raid");
```

The parser is intentionally strict so malformed component IDs fail early in tests instead of routing to the wrong feature in production.

## Pipeline behavior

The pipeline executes modules by `Order`. A module must return one of the supported result shapes:

- unhandled: continue to next module;
- handled with no response: stop;
- handled with response plan: write response and stop;
- failed: record failure and optionally write a safe failure response.

Configuration controls safe fallback behavior:

```json
{
  "DiscSharp": {
    "Interactions": {
      "Pipeline": {
        "RespondToUnhandledInteractions": true,
        "RespondToModuleFailures": true,
        "UnhandledResponseContent": "That interaction is not handled by this application module.",
        "FailureResponseContent": "Something went wrong while handling that interaction."
      }
    }
  }
}
```

## Response writer

`IInteractionResponseWriter` is the infrastructure port that turns an `InteractionResponsePlan` into Discord REST calls. Keep it out of application modules. The module returns intent; infrastructure performs transport.

## Raid Manager pack

`DiscSharp.InteractionPacks.RaidManager` contains a real module skeleton that depends on `IRaidManagerInteractionService`.

Use it as the pattern for feature packs:

- no direct Discord REST in the module;
- parse `DiscordComponentCustomId`;
- call an app service port;
- return an interaction result;
- register through an Autofac module.

## Music Player pack

`DiscSharp.InteractionPacks.MusicPlayer` follows the same shape for playback controls. Actual playback and voice transport belong behind service ports, not in the interaction module.

## Testing modules

A module test should construct an envelope and a fake service, then assert the returned `InteractionModuleResult`.

```csharp
var module = new RaidManagerInteractionModule(fakeRaidService);
var envelope = TestInteractions.Component("raid:join;raidId=123");

var result = await module.HandleAsync(envelope, CancellationToken.None);

Assert.True(result.Handled);
Assert.NotNull(result.ResponsePlan);
```
