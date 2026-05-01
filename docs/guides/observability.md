# Observability

DiscSharp’s production story depends on logs, traces, and metrics. Discord failures are often timing-sensitive: rate limits, Gateway reconnects, interaction response deadlines, and handler latency. The library should expose those signals instead of making app owners reverse engineer them.

## Logging

Use Serilog as the application logging pipeline and inject `ILogger<T>` into services and handlers.

Recommended structured properties:

| Area | Properties |
| --- | --- |
| REST | route, method, status code, Discord error code, rate-limit bucket, retry-after, global/scope |
| Gateway | event name, sequence, opcode, handler name, payload type, dispatch duration |
| Interactions | interaction id, application id, guild id, channel id, user id, module name, response kind |
| Components | custom id module, action, argument names, parse status |

Never log bot tokens or interaction tokens.

## Tracing

Current telemetry helper types:

- `DiscordRestTelemetry`
- `GatewayDispatchTelemetry`
- `InteractionPipelineTelemetry`

These should back `ActivitySource` instances and emit spans around:

- HTTP request execution;
- interaction callback/followup requests;
- Gateway dispatch orchestration;
- individual handler invocation;
- interaction pipeline/module execution.

Recommended activity names:

```text
DiscSharp.Rest.Request
DiscSharp.Rest.Interaction.CreateResponse
DiscSharp.Gateway.Dispatch
DiscSharp.Gateway.Handler
DiscSharp.Interactions.Pipeline
DiscSharp.Interactions.Module
```

Recommended tags:

```text
discord.api.version
discord.route
discord.gateway.event_name
discord.gateway.sequence
discsharp.handler.name
discsharp.handler.failure_policy
discsharp.interaction.kind
discsharp.interaction.module
discsharp.component.module
discsharp.component.action
```

## Metrics

Minimum metric names:

```text
discsharp.rest.requests
discsharp.rest.request.duration
discsharp.rest.rate_limits
discsharp.gateway.dispatches
discsharp.gateway.dispatch.duration
discsharp.gateway.handler.duration
discsharp.gateway.handler.failures
discsharp.interactions.handled
discsharp.interactions.unhandled
discsharp.interactions.failures
discsharp.interactions.module.duration
```

## Serilog/OpenTelemetry host sketch

```csharp
builder.Host.UseSerilog((context, services, logger) => logger
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("DiscSharp.Rest")
        .AddSource("DiscSharp.Gateway")
        .AddSource("DiscSharp.Interactions"))
    .WithMetrics(metrics => metrics
        .AddMeter("DiscSharp.Rest")
        .AddMeter("DiscSharp.Gateway")
        .AddMeter("DiscSharp.Interactions"));
```

The exact source/meter names should stay stable once package naming is finalized.

## Alert-worthy conditions

Production apps should alert on repeated `401 Unauthorized` or `403 Forbidden` REST errors, recurring global rate limits, handler timeouts, missed interaction response deadlines, heartbeat ACK failures, repeated reconnects that cannot resume, `DisallowedIntents`, and unhandled interaction spikes after deployment.
