# Current Status and Roadmap

This document is intentionally blunt. DiscSharp should be transparent about what is implemented, what is in progress, and what is not done yet.

## Implemented in the current rollup

### REST/API alignment

- `DiscSharp.Rest` project.
- Discord API v10 base configuration.
- Discord-style User-Agent option validation.
- Route factories for interaction callback/original/followup endpoints.
- Discord query string builder with repeated-key arrays.
- Discord REST JSON options.
- Rate-limit header parsing model.
- REST error and exception model.
- Interaction callback/followup REST client.
- Interaction response payload factories.
- Webhook message payload/message models.
- Component models for buttons, selects, text inputs, labels, and modal file uploads.
- Autofac module.
- REST unit tests.

### Gateway

- Typed dispatch envelope.
- Multi-handler catalog.
- Multi-handler orchestrator.
- Ordered fan-out.
- Sequential and parallel-by-order-band modes.
- Per-handler timeout.
- Continue/stop failure policy.
- Gateway dispatch telemetry seam.
- `INTERACTION_CREATE` Gateway-to-interaction adapter pattern.
- Gateway orchestration tests.

### Application interactions

- Transport-neutral interaction envelope.
- Interaction kind classification.
- Interaction module interface.
- Interaction pipeline.
- Response plan abstraction.
- Response writer port.
- Custom ID parser/builder.
- Interaction pipeline options.
- Interaction telemetry seam.
- Application tests.

### Feature packs

- Raid Manager interaction pack skeleton.
- Music Player interaction pack skeleton.
- App service ports for pack behavior.
- Autofac modules for packs.

## In progress / next priority

1. **REST endpoint expansion**: channels, messages, guilds, users, application commands, webhooks beyond interaction followups, audit-log reason support, multipart file upload.
2. **Command registration**: global commands, guild commands, command diffing, idempotent command sync, structured validation before hitting Discord.
3. **Interaction pipeline hardening**: concrete REST response writer, envelope factories for current Gateway DTOs, HTTP ingress path, signature verification, module preconditions, pipeline middleware without MediatR.
4. **Gateway lifecycle**: identify/resume state machine, heartbeat ACK watchdog, reconnect policy, session start limit handling, shard coordination, privileged-intent diagnostics.
5. **Components V2**: factories for sections, containers, separators, text display, media gallery, thumbnail, file display, radio group, checkbox group, checkbox; payload restrictions around `IS_COMPONENTS_V2`.
6. **Voice and music**: voice gateway lifecycle, UDP transport, Opus pipeline, FFmpeg boundary, music queue domain model, resilient playback controls.
7. **Samples**: realtime console smoke tests, ASP.NET webhook ingress sample, Worker Service bot sample, Blazor AppHost dashboard, TypeScript/web-surface bridge sample.

## Not implemented yet

- Full Discord REST route coverage.
- Full Gateway event model coverage.
- Voice pipeline.
- Sharding.
- Command sync engine.
- Multipart upload client.
- OAuth2 flows.
- Persistent state model.
- Production package/nuspec metadata.
- Public website docs generator.

## Quality gates before public preview

- All public APIs documented with XML comments.
- Full solution builds cleanly in Release.
- Tests pass on Windows and Linux.
- REST endpoint clients covered by route/payload tests.
- Gateway lifecycle smoke test passes against Discord.
- Interaction callback/followup smoke test passes against Discord.
- No token or interaction token leakage in logs.
- OpenTelemetry source/meter names stabilized.
- README shows a complete runnable app.
- Package names/versioning decided.

## Standard against existing libraries

DiscSharp does not need to claim dominance in every sentence. It needs to prove it with engineering: current Discord API alignment, cleaner production composition, stronger observability, safer handler orchestration, modern component/modal support, and docs that explain architecture and operations, not just snippets.
