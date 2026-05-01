# Getting Started

This is the second stop in the onboarding path after [README.md](../../README.md): the README shows the map, and this guide turns that map into a host you can actually run.

It wires the current DiscSharp surface into a real .NET host-style application and assumes the projects are referenced from the current solution.

## Requirements

- .NET 10 SDK.
- C# latest language version.
- Autofac as the application container.
- Serilog for the app logging pipeline.
- OpenTelemetry for traces and metrics.
- A Discord application/bot token for live REST/Gateway smoke tests.

## Step 1: reference the projects

```xml
<ItemGroup>
  <ProjectReference Include="..\DiscSharp.Rest\DiscSharp.Rest.csproj" />
  <ProjectReference Include="..\DiscSharp.Gateway\DiscSharp.Gateway.csproj" />
  <ProjectReference Include="..\DiscSharp.Application\DiscSharp.Application.csproj" />
  <ProjectReference Include="..\DiscSharp.InteractionPacks.RaidManager\DiscSharp.InteractionPacks.RaidManager.csproj" />
  <ProjectReference Include="..\DiscSharp.InteractionPacks.MusicPlayer\DiscSharp.InteractionPacks.MusicPlayer.csproj" />
</ItemGroup>
```

Use only the packs your app needs. The smaller the graph, the less archaeology your future self has to perform.

## Step 2: wire Autofac

```csharp
using Autofac;
using DiscSharp.Application.DependencyInjection;
using DiscSharp.Gateway.DependencyInjection;
using DiscSharp.InteractionPacks.MusicPlayer;
using DiscSharp.InteractionPacks.RaidManager;
using DiscSharp.Rest.DependencyInjection;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var builder = new ContainerBuilder();

builder.RegisterInstance<IConfiguration>(configuration)
    .As<IConfiguration>()
    .SingleInstance();

builder.RegisterModule(new DiscordRestModule(options =>
{
    options.UserAgent = "DiscordBot (https://github.com/your-org/your-bot, 1.0.0)";
    options.ApiVersion = 10;
}));

builder.RegisterModule(new GatewayOrchestrationModule(configuration));
builder.RegisterModule(new InteractionPipelineModule(configuration));

// Feature packs. Register the app services they depend on in your composition root.
builder.RegisterModule<RaidManagerInteractionPackModule>();
builder.RegisterModule<MusicPlayerInteractionPackModule>();

await using var container = builder.Build();
```

The real host should also register pack service ports such as `IRaidManagerInteractionService` and `IMusicPlayerInteractionService`. Those are application-owned services, not Discord infrastructure.

## Step 3: configure the host

```json
{
  "DiscSharp": {
    "Gateway": {
      "DispatchOrchestration": {
        "ExecutionMode": "Sequential",
        "HandlerTimeout": "00:00:15",
        "LogUnhandledDispatches": true,
        "MaxParallelHandlersPerOrderBand": 16
      }
    },
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

## Step 4: make your first REST call

```csharp
using DiscSharp.Rest.Interactions;
using DiscSharp.Rest.Primitives;

var client = container.Resolve<IDiscordInteractionRestClient>();

await client.CreateInteractionResponseAsync(
    interactionId: new DiscordSnowflake("123456789012345678"),
    interactionToken: interactionToken,
    payload: DiscordInteractionResponsePayload.Message("Ready.", ephemeral: true),
    withResponse: false,
    cancellationToken: cancellationToken);
```

Use `DeferChannelMessage(ephemeral: true)` when your handler cannot complete immediately.

## Step 5: add an interaction module

```csharp
using DiscSharp.Application.Interactions;

public sealed class HelpInteractionModule : IDiscordInteractionModule
{
    public string ModuleName => "help";

    public int Order => 0;

    public bool CanHandle(DiscordInteractionEnvelope interaction) =>
        interaction.Kind is DiscordInteractionKind.ApplicationCommand &&
        string.Equals(interaction.Name, "help", StringComparison.OrdinalIgnoreCase);

    public ValueTask<InteractionModuleResult> HandleAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(
            InteractionModuleResult.Respond(
                InteractionResponsePlan.Message("DiscSharp is online.", ephemeral: true)));
    }
}
```

Register modules as `IDiscordInteractionModule` in Autofac. Feature packs already do this for their own modules.

## Step 6: run the build and tests

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## Step 7: what to build next

1. Register real `IRaidManagerInteractionService` / `IMusicPlayerInteractionService` implementations.
2. Adapt your typed `INTERACTION_CREATE` DTO into `DiscordInteractionEnvelope` through `IInteractionEnvelopeFactory<TInteractionCreateEvent>`.
3. Use `InteractionCreateGatewayDispatchHandler<TInteractionCreateEvent>` to bridge Gateway dispatch into the interaction pipeline.
4. Add smoke tests against a dev Discord server.
5. Add OpenTelemetry exporters and Serilog sinks in the host.
