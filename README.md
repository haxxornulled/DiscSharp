# DiscSharp

DiscSharp is a .NET 10 / C# Discord library for people who enjoy strong typing, explicit composition roots, and not discovering architecture problems at 2:00 AM in production.

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-haxxornulled%2FDiscSharp-181717?logo=github&logoColor=white)](https://github.com/haxxornulled/DiscSharp)
[![Docs](https://img.shields.io/badge/docs-onboarding-0A7EA4)](docs/README.md)

It is intentionally opinionated: REST primitives are typed, Gateway dispatch is orchestrated, interactions flow through an application pipeline, and the whole thing is built to be observable instead of mysterious.

## Why it exists

The goal is not to make Discord development louder. It is to make it boring in production, testable in local builds, and legible to the next engineer who opens the repo after a long weekend.

That means:

- Autofac-first composition
- typed REST, Gateway, and interaction surfaces
- multi-handler dispatch orchestration
- interaction packs instead of copy-pasted bot spaghetti
- observability seams for logging, metrics, and tracing
- tests that verify behavior instead of vibes

## Start here

If you want the shortest path from clone to working host, follow this order:

1. Read [docs/README.md](docs/README.md) for the doc map and project shape.
2. Follow [docs/guides/getting-started.md](docs/guides/getting-started.md) for the host wiring.
3. Check [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md) so you know what is implemented and what is still on the bench.

## Quickstart

The quickstart below is the same path, just compressed into a single page so you do not have to hop around if you are already in the zone.

### 1. Install and reference

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

### 2. Wire Autofac

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

### 3. Configure the host

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

### 4. Add a first interaction module

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

### 5. Build and test

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## NuGet

The package build is alive locally: `DiscSharp` is the umbrella package, and the layer packages (`DiscSharp.Rest`, `DiscSharp.Gateway`, `DiscSharp.Application`) stay available for people who want the components instead of the whole tower.

When the packages are published, the install shape is the usual:

```powershell
dotnet add package DiscSharp
```

For local packing, use:

```powershell
dotnet pack .\DiscSharp.slnx -c Release
```

The packages land in `artifacts/packages`.

Publishing from GitHub Actions uses Trusted Publishing, so the workflow does not need a long-lived NuGet API key. The release job is in [`.github/workflows/build.yml`](.github/workflows/build.yml), and nuget.org needs a matching Trusted Publishing policy for that workflow file and the `production` environment.

If you want the lower-level pieces, the repo also ships as individual packages:

- `DiscSharp.Rest` for Discord API v10 routes, interaction webhook shapes, and rate-limit primitives
- `DiscSharp.Gateway` for typed Gateway dispatch orchestration
- `DiscSharp.Application` for transport-neutral interaction pipelines
- `DiscSharp.InteractionPacks.RaidManager` and `DiscSharp.InteractionPacks.MusicPlayer` as feature-pack examples
- tests that lock down routes, payloads, orchestration, and interaction behavior

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for the shortest path to making changes without accidentally fighting the architecture.

## Current status

See [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md).
