# Why DiscSharp

DiscSharp exists because high-throughput Discord applications deserve the same engineering discipline used in serious .NET services: clean boundaries, DI composition, lifecycle management, telemetry, failure isolation, and tests that verify behavior.

This is not a “Discord.Net is bad” document. Discord.Net is a long-running community library with a broad feature surface. DiscSharp’s goal is different: provide a modern, enterprise-first .NET 10/C# library where the architecture is as important as endpoint coverage.

## The standard

DiscSharp should be the library a .NET architect can put into a production bot, SaaS integration, community automation platform, or embedded Discord UI scenario and still sleep at night.

That means:

- **Autofac-first composition.** Framework registrations can originate from `IServiceCollection`, but DiscSharp’s own composition root should be Autofac modules and explicit registrations.
- **Clean Architecture.** Gateway, REST, and Discord transport DTOs do not own application behavior.
- **Typed APIs.** Routes, snowflakes, interaction responses, components, rate-limit headers, and webhook messages are typed models.
- **Observability by design.** Activity/meter/logging seams exist where production operators actually need them.
- **No single-handler bottleneck.** Gateway dispatch supports multiple handlers per typed event with ordering, failure policy, and execution-mode control.
- **Interaction packs.** App scenarios like Raid Manager and Music Player should ship as composable modules, not copy/pasted bot code.
- **Tests are part of the public contract.** Serialization, routing, orchestration ordering, failure behavior, and parser rules are locked down.

## Positioning against existing docs and libraries

Existing Discord.Net documentation is heavily oriented around `IServiceCollection`, `DiscordSocketClient`, `InteractionService`, attribute modules, transient command modules, and helper methods such as `RespondAsync()` and `FollowupAsync()` inside framework-managed modules.

DiscSharp intentionally documents a different shape:

| Topic | Common Discord.Net-style approach | DiscSharp direction |
| --- | --- | --- |
| Composition | `ServiceCollection` + `IServiceProvider` examples | Autofac modules as first-class product surface |
| Interaction handling | Attribute-discovered modules and helper base classes | Transport-neutral `IDiscordInteractionModule` pipeline |
| Component routing | Attribute patterns and wildcard custom IDs | Explicit `DiscordComponentCustomId` parser/builder and module routing |
| Gateway dispatch | Event callbacks or one-off handlers | Typed dispatch envelope + multi-handler orchestration |
| Failure behavior | Application-defined ad hoc try/catch | Handler-level failure policy and orchestration result model |
| Observability | Mostly up to the app | Built-in telemetry seams for REST, Gateway, and interaction pipelines |
| REST alignment | Often hidden behind client methods | Public route/query/rate-limit primitives so behavior is inspectable and testable |
| Enterprise testing | App-specific | Library-level tests for payloads, routes, headers, ordering, and failure modes |

The message is not “we are louder.” The message is “we are more explicit.” DiscSharp’s API surface should make the correct architecture the easiest architecture.

## What better documentation means here

The docs should not only show snippets. They should answer production questions:

- Where does this service live in the architecture?
- What owns lifetime and disposal?
- What is safe to use in app/domain code?
- Where is Discord API weirdness hidden or surfaced?
- What gets logged, traced, and measured?
- What happens when a handler fails?
- How do I test this without a real Discord connection?
- Which features are done, and which are still on the roadmap?

Every public feature should eventually have a short explanation, complete minimal usage, production wiring, failure/observability notes, tests, and Discord compatibility notes.

## Non-goals

DiscSharp should not become an anemic JSON endpoint mirror, a global service locator, a MediatR-first bot framework, an attribute-only interaction framework, or a library that hides all rate-limit/Gateway/interaction lifecycle details until production breaks.

## Near-term product story

1. Use `DiscSharp.Rest` for Discord API v10 route/query/rate-limit primitives and interaction callback/followup responses.
2. Use `DiscSharp.Gateway` to fan out typed Gateway dispatch events safely.
3. Use `DiscSharp.Application` to route interactions to app-specific modules.
4. Use interaction packs as examples of feature modules that keep Discord transport details out of business services.
5. Wire everything through Autofac.
6. Observe everything through Serilog and OpenTelemetry.
