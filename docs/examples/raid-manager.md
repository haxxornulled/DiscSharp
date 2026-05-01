# Example: Raid Manager Interaction Flow

This example shows how the current pieces fit together for a Raid Manager app scenario.

## Goal

A user clicks a `Join Raid` button. DiscSharp should receive `INTERACTION_CREATE` from Gateway, adapt the typed Gateway DTO into `DiscordInteractionEnvelope`, dispatch through the interaction pipeline, route to the Raid Manager pack, call the application service, return an ephemeral response or update message plan, and write the response through REST infrastructure.

## Component custom ID

```csharp
var customId = DiscordComponentCustomId.Create(
    module: "raid",
    action: "join",
    arguments: new Dictionary<string, string>
    {
        ["raidId"] = raid.Id.ToString(CultureInfo.InvariantCulture)
    });
```

## Button

```csharp
var button = DiscordComponent.Button(
    customId: customId.ToString(),
    label: "Join Raid",
    style: DiscordButtonStyle.Success);
```

## App service port

```csharp
public interface IRaidManagerInteractionService
{
    ValueTask<RaidManagerInteractionResult> JoinRaidAsync(
        RaidInteractionRequest request,
        CancellationToken cancellationToken);
}
```

The implementation belongs to the app/domain layer, not the Gateway or REST layer.

## Registration

```csharp
builder.RegisterType<SqlRaidManagerInteractionService>()
    .As<IRaidManagerInteractionService>()
    .InstancePerLifetimeScope();

builder.RegisterModule<RaidManagerInteractionPackModule>();
```

## Test shape

```csharp
var service = Substitute.For<IRaidManagerInteractionService>();
service.JoinRaidAsync(Arg.Any<RaidInteractionRequest>(), Arg.Any<CancellationToken>())
    .Returns(RaidManagerInteractionResult.Success("You joined the raid."));

var module = new RaidManagerInteractionModule(service);
var envelope = TestInteractionEnvelope.Component("raid:join;raidId=123");

var result = await module.HandleAsync(envelope, CancellationToken.None);

Assert.True(result.Handled);
Assert.Contains("joined", result.ResponsePlan!.Content, StringComparison.OrdinalIgnoreCase);
```
