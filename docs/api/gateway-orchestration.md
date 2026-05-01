# Gateway Orchestration

`DiscSharp.Gateway` contains the multi-handler dispatch orchestration surface. A typed Discord Gateway event can fan out to multiple handlers without turning the Gateway loop into a fragile pile of callbacks.

## Core concepts

| Type | Purpose |
| --- | --- |
| `GatewayDispatchEnvelope` | Transport-neutral wrapper for event name, payload, sequence, and metadata. |
| `IDiscordGatewayDispatchHandler` | A handler for one typed dispatch payload. |
| `IGatewayDispatchHandlerCatalog` | Finds handlers matching an envelope. |
| `IDiscordGatewayDispatchOrchestrator` | Executes all matching handlers according to policy. |
| `GatewayDispatchOrchestrationOptions` | Controls execution mode, timeout, and unhandled-event logging. |

## Handler shape

```csharp
using DiscSharp.Gateway.Dispatch.Orchestration;

public sealed class AuditInteractionCreateHandler : IDiscordGatewayDispatchHandler
{
    public string HandlerName => nameof(AuditInteractionCreateHandler);
    public string EventName => "INTERACTION_CREATE";
    public Type PayloadType => typeof(MyInteractionCreateEvent);
    public int Order => -100;
    public GatewayHandlerFailurePolicy FailurePolicy => GatewayHandlerFailurePolicy.Continue;

    public bool CanHandle(GatewayDispatchEnvelope envelope) =>
        envelope.EventName == EventName && envelope.Payload is MyInteractionCreateEvent;

    public async ValueTask<GatewayHandlerExecutionResult> HandleAsync(
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        var payload = (MyInteractionCreateEvent)envelope.Payload;
        await WriteAuditEntryAsync(payload, cancellationToken).ConfigureAwait(false);
        return GatewayHandlerExecutionResult.Success(HandlerName);
    }
}
```

## Ordering

Handlers are sorted by `Order` ascending.

| Order band | Use |
| --- | --- |
| `-1000` to `-100` | validation, audit, low-level safety checks |
| `-99` to `99` | normal application behavior |
| `100` to `999` | secondary notifications, projections, metrics |
| `1000+` | best-effort integrations |

## Failure policies

| Policy | Meaning |
| --- | --- |
| `Continue` | Log/record failure and continue to later handlers. |
| `StopPipeline` | Stop dispatching this envelope after failure. |

Use `StopPipeline` only for handlers that protect correctness or safety. Most analytics, projections, and notifications should use `Continue`.

## Execution modes

`GatewayHandlerExecutionMode.Sequential` runs handlers one at a time in order.

`GatewayHandlerExecutionMode.ParallelByOrderBand` allows handlers with the same `Order` to run concurrently while preserving order between bands. Use it only when handlers are independent and thread-safe.

## Configuration

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
    }
  }
}
```

## Dispatching

```csharp
var orchestrator = container.Resolve<IDiscordGatewayDispatchOrchestrator>();

GatewayDispatchOrchestrationResult result = await orchestrator.DispatchAsync(
    envelope,
    cancellationToken);

if (!result.Succeeded)
{
    logger.LogWarning(
        "Gateway dispatch completed with failures. EventName={EventName} Sequence={Sequence} FailureCount={FailureCount}",
        envelope.EventName,
        envelope.Sequence,
        result.Failures.Count);
}
```

## Interaction bridge

`InteractionCreateGatewayDispatchHandler<TInteractionCreateEvent>` adapts a typed `INTERACTION_CREATE` Gateway event into `IDiscordInteractionPipeline`.

```csharp
builder.RegisterType<MyInteractionEnvelopeFactory>()
    .As<IInteractionEnvelopeFactory<MyInteractionCreateEvent>>()
    .SingleInstance();

builder.RegisterType<InteractionCreateGatewayDispatchHandler<MyInteractionCreateEvent>>()
    .As<IDiscordGatewayDispatchHandler>()
    .SingleInstance();
```

This keeps Gateway DTOs out of application modules and keeps application modules testable without a WebSocket.

## Observability

The orchestrator records handler execution status and duration through `GatewayDispatchTelemetry`. Handler names, event names, payload types, sequence numbers, and failure policies should be included in structured logs and telemetry tags.

## Testing strategy

Test multiple matching handlers, ordering, `Continue`, `StopPipeline`, timeout, cancellation, and no-handler behavior without a real Discord connection.
