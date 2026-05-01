# Integration notes

## Existing gateway receive loop

Where the previous pass currently invokes a single typed handler/subscription, create a `GatewayDispatchEnvelope` and call:

```csharp
await gatewayDispatchOrchestrator.DispatchAsync(envelope, stoppingToken).ConfigureAwait(false);
```

Recommended construction:

```csharp
var envelope = new GatewayDispatchEnvelope(
    eventName: dispatch.EventName,
    payload: typedPayload,
    payloadType: typedPayload.GetType(),
    sequenceNumber: dispatch.SequenceNumber,
    receivedAt: timeProvider.GetUtcNow(),
    shardId: shardId,
    correlationId: correlationContext.CorrelationId);
```

If you already have a typed dispatch wrapper, add an extension method there instead of duplicating construction in the receive loop.

## Autofac root module

Register these modules from your existing root module:

```csharp
builder.RegisterModule(new GatewayOrchestrationModule(configuration));
builder.RegisterModule(new InteractionPipelineModule(configuration));
builder.RegisterModule<RaidManagerInteractionPackModule>();
builder.RegisterModule<MusicPlayerInteractionPackModule>();
```

Also register application implementations for:

```csharp
IRaidManagerInteractionService
IMusicPlayerInteractionService
IInteractionResponseWriter
IInteractionEnvelopeFactory<TInteractionCreateEvent>
```

## Interaction response writer

`IInteractionResponseWriter` belongs in infrastructure because it calls Discord REST. It should use the existing REST client, typed endpoint surface, retry/backoff, logging, and telemetry. Do not let interaction modules call REST directly.

## DTO-specific interaction factory

Implement `IInteractionEnvelopeFactory<TInteractionCreateEvent>` in the project that knows the current Discord DTO shape. That class maps Discord interaction type integers/enums to `DiscordInteractionKind`, extracts IDs, command name, custom ID, token, and raw payload.

## Realtime console smoke test

After merge, add a smoke option that logs registered gateway handlers using:

```csharp
var catalog = scope.Resolve<IGatewayDispatchHandlerCatalog>();
foreach (var descriptor in catalog.DescribeHandlers())
{
    logger.LogInformation(
        "Gateway handler: {HandlerName} event={EventName} payload={PayloadType} order={Order} failurePolicy={FailurePolicy}",
        descriptor.HandlerName,
        descriptor.EventName,
        descriptor.PayloadType.FullName,
        descriptor.Order,
        descriptor.FailurePolicy);
}
```

Then trigger a slash command/button/modal in a test Discord guild and verify:

- `INTERACTION_CREATE` enters gateway orchestration.
- `InteractionCreateGatewayDispatchHandler<T>` invokes the pipeline.
- The expected pack module handles the interaction.
- REST response writer acknowledges within Discord's interaction window.
- OTel metrics show gateway dispatch + interaction pipeline execution.
