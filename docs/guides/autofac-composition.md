# Autofac Composition

DiscSharp is Autofac-first. That does not mean the wider app can never touch `IServiceCollection`; it means DiscSharp’s product-level composition is not built around Microsoft DI as the root container.

## Rule

Use `IServiceCollection` only when a framework package requires it or when host infrastructure starts there. Bridge those registrations into Autofac, then keep DiscSharp registrations in Autofac modules.

## Modules currently provided

| Module | Project | Registers |
| --- | --- | --- |
| `DiscordRestModule` | `DiscSharp.Rest` | `DiscordApiOptions`, `IOptions<DiscordApiOptions>`, `HttpClient`, `DiscordRestTelemetry`, `IDiscordInteractionRestClient` |
| `GatewayOrchestrationModule` | `DiscSharp.Gateway` | gateway orchestration options, telemetry, handler catalog, dispatch orchestrator |
| `InteractionPipelineModule` | `DiscSharp.Application` | interaction pipeline options, telemetry, `IDiscordInteractionPipeline` |
| `RaidManagerInteractionPackModule` | `DiscSharp.InteractionPacks.RaidManager` | Raid Manager interaction module |
| `MusicPlayerInteractionPackModule` | `DiscSharp.InteractionPacks.MusicPlayer` | Music Player interaction module |

## Approved composition root shape

```csharp
var builder = new ContainerBuilder();

builder.RegisterInstance(configuration)
    .As<IConfiguration>()
    .SingleInstance();

builder.RegisterModule(new DiscordRestModule(options =>
{
    options.UserAgent = "DiscordBot (https://github.com/your-org/your-bot, 1.0.0)";
}));

builder.RegisterModule(new GatewayOrchestrationModule(configuration));
builder.RegisterModule(new InteractionPipelineModule(configuration));

builder.RegisterType<MyRaidManagerInteractionService>()
    .As<IRaidManagerInteractionService>()
    .SingleInstance();

builder.RegisterModule<RaidManagerInteractionPackModule>();
```

## Handler registration

```csharp
builder.RegisterType<MyGuildCreateHandler>()
    .As<IDiscordGatewayDispatchHandler>()
    .SingleInstance();

builder.RegisterType<MyInteractionModule>()
    .As<IDiscordInteractionModule>()
    .SingleInstance();
```

Prefer singleton stateless handlers/modules. Put mutable state behind explicit application services with clear concurrency rules.

## Testing rule

Do not manually construct a graph in tests when the behavior under test is composition-sensitive. For integration-style tests, build a real Autofac container and resolve the public service.

## What not to do

Do not use this as the product composition pattern:

```csharp
var services = new ServiceCollection();
services.AddSingleton<IDiscordInteractionModule, MyModule>();
var provider = services.BuildServiceProvider();
```

That recreates the loose framework-default composition model DiscSharp is intentionally avoiding.
