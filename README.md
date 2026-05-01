# DiscSharp

DiscSharp is a .NET 10 / C# Discord library for people who enjoy strong typing, explicit composition roots, and not discovering architecture problems at 2:00 AM in production.

It is intentionally opinionated: REST primitives are typed, Gateway dispatch is orchestrated, interactions flow through an application pipeline, and the whole thing is built to be observable instead of mysterious.

## Why it exists

The goal is not to make Discord development louder. It is to make it boring in production, testable in CI, and legible to the next engineer who opens the repo after a long weekend.

That means:

- Autofac-first composition
- typed REST, Gateway, and interaction surfaces
- multi-handler dispatch orchestration
- interaction packs instead of copy-pasted bot spaghetti
- observability seams for logging, metrics, and tracing
- tests that verify behavior instead of vibes

## Quickstart

If you just want to get DiscSharp wired into a host, start here and then use the deeper docs for the parts that deserve more ceremony.

1. Install the `.NET 10 SDK`.
2. Reference the projects you need.
3. Register the DiscSharp modules in Autofac.
4. Put your Discord settings in configuration.
5. Run the solution with restore/build/test.

### Project references

```xml
<ItemGroup>
  <ProjectReference Include="..\DiscSharp.Rest\DiscSharp.Rest.csproj" />
  <ProjectReference Include="..\DiscSharp.Gateway\DiscSharp.Gateway.csproj" />
  <ProjectReference Include="..\DiscSharp.Application\DiscSharp.Application.csproj" />
  <ProjectReference Include="..\DiscSharp.InteractionPacks.RaidManager\DiscSharp.InteractionPacks.RaidManager.csproj" />
  <ProjectReference Include="..\DiscSharp.InteractionPacks.MusicPlayer\DiscSharp.InteractionPacks.MusicPlayer.csproj" />
</ItemGroup>
```

Use only the packs your app actually needs. Your bot does not need to drag around the whole warehouse if it only ordered one shelf.

### Minimal Autofac composition

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

builder.RegisterModule<RaidManagerInteractionPackModule>();
builder.RegisterModule<MusicPlayerInteractionPackModule>();

await using var container = builder.Build();
```

The application host still owns the concrete app services behind those packs, such as `IRaidManagerInteractionService` and `IMusicPlayerInteractionService`.

### appsettings.json

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

### First interaction module

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

Register modules as `IDiscordInteractionModule` in Autofac. The feature packs already do this for their own modules.

### Build and test

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## What's in the box

- `DiscSharp.Rest` for Discord API v10 routes, interaction webhook shapes, and rate-limit primitives
- `DiscSharp.Gateway` for typed Gateway dispatch orchestration
- `DiscSharp.Application` for transport-neutral interaction pipelines
- `DiscSharp.InteractionPacks.RaidManager` and `DiscSharp.InteractionPacks.MusicPlayer` as feature-pack examples
- tests that lock down routes, payloads, orchestration, and interaction behavior

## Docs

Start here:

1. [docs/README.md](docs/README.md)
2. [docs/architecture/why-discsharp.md](docs/architecture/why-discsharp.md)
3. [docs/guides/getting-started.md](docs/guides/getting-started.md)
4. [docs/api/rest.md](docs/api/rest.md)
5. [docs/api/gateway-orchestration.md](docs/api/gateway-orchestration.md)
6. [docs/api/interactions.md](docs/api/interactions.md)
7. [docs/api/components-and-modals.md](docs/api/components-and-modals.md)
8. [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md)

## Current status

See [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md).
