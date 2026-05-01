# DiscSharp Documentation

DiscSharp is being built as an enterprise-grade .NET 10/C# Discord platform library: typed REST primitives, interaction callback/followup support, Gateway dispatch orchestration, an interaction pipeline, component/modal models, Autofac modules, OpenTelemetry hooks, and tests.

If you arrived from [README.md](../README.md), this page is the doc map that keeps the onboarding path linear instead of requiring psychic powers.

## Start here

| Document | Purpose |
| --- | --- |
| [Why DiscSharp](./architecture/why-discsharp.md) | Product intent, engineering bar, and how DiscSharp positions itself against existing libraries without empty bragging. |
| [Getting Started](./guides/getting-started.md) | Wire the current projects into a real app. |
| [Autofac Composition](./guides/autofac-composition.md) | Approved DI shape: Autofac-first, `IServiceCollection` only where framework integration requires it. |
| [REST API](./api/rest.md) | Use API v10 routes, interaction callback/followup endpoints, query strings, rate-limit headers, and error handling. |
| [Gateway Orchestration](./api/gateway-orchestration.md) | Use ordered multi-handler Gateway dispatch with failure policies and telemetry. |
| [Interactions Pipeline](./api/interactions.md) | Build application modules for commands, components, modals, autocomplete, Raid Manager, and Music Player scenarios. |
| [Components and Modals](./api/components-and-modals.md) | Build buttons, selects, modal labels, text inputs, and file uploads. |
| [Observability](./guides/observability.md) | Logging, tracing, metrics, and production alert guidance. |
| [Testing](./guides/testing.md) | Expected tests for routes, payloads, parsers, orchestration, and handlers. |
| [Status and Roadmap](./architecture/status-and-roadmap.md) | What is done, what is next, and what is not implemented yet. |

## Current project map

```text
src/
  DiscSharp.Rest/                         Discord HTTP API primitives and interaction REST client
  DiscSharp.Gateway/                      Gateway dispatch orchestration and interaction gateway adapter
  DiscSharp.Application/                  Transport-neutral interaction pipeline
  DiscSharp.InteractionPacks.RaidManager/ Feature pack skeleton for raid flows
  DiscSharp.InteractionPacks.MusicPlayer/ Feature pack skeleton for music controls

tests/
  DiscSharp.Rest.Tests/
  DiscSharp.Gateway.Tests/
  DiscSharp.Application.Tests/
```

## Design promise

DiscSharp should feel boring in production and excellent under stress:

- Clean Architecture boundaries;
- Autofac-first composition;
- typed models instead of dictionary soup;
- OpenTelemetry seams at important boundaries;
- Serilog-compatible structured logging;
- Discord API details captured in reusable primitives;
- tests for route generation, payload shape, rate-limit parsing, orchestration, and interaction routing;
- no MediatR-first composition, no service-locator handlers, no “just new it up in infrastructure” shortcuts.

## Discord API alignment

The current pass targets Discord API v10. It documents/encodes these platform rules:

- REST base path is `https://discord.com/api/v10/`.
- HTTP requests must send a valid `User-Agent`.
- Array query parameters use repeated keys unless an endpoint says otherwise.
- REST rate-limit behavior must be driven from response headers and bucket metadata.
- Gateway dispatch payloads use opcode `0`, event name `t`, sequence `s`, and data `d`.
- Interactions received over Gateway still respond over HTTP via `/interactions/{interaction.id}/{interaction.token}/callback`.
- Interaction tokens are used for followups/original-response edits for a limited lifetime.
- Components V2 requires `IS_COMPONENTS_V2` and changes how message content is represented.

## Naming note

These docs say “current surface” intentionally. DiscSharp is not pretending every Discord route, Gateway event, voice scenario, and component family is complete. The point is to expose the stable shape, make usage obvious, and make the remaining work visible.
