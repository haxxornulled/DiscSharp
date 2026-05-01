namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Base class for strongly typed Discord gateway dispatch handlers.
/// </summary>
/// <typeparam name="TPayload">The typed gateway dispatch payload.</typeparam>
public abstract class DiscordGatewayDispatchHandler<TPayload> : IDiscordGatewayDispatchHandler
    where TPayload : class
{
    /// <inheritdoc />
    public virtual string HandlerName => GetType().Name;

    /// <inheritdoc />
    public abstract string EventName { get; }

    /// <inheritdoc />
    public Type PayloadType => typeof(TPayload);

    /// <inheritdoc />
    public virtual int Order => 0;

    /// <inheritdoc />
    public virtual GatewayHandlerFailurePolicy FailurePolicy => GatewayHandlerFailurePolicy.Continue;

    /// <inheritdoc />
    public virtual bool CanHandle(GatewayDispatchEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        return string.Equals(envelope.EventName, EventName, StringComparison.Ordinal)
            && envelope.Payload is TPayload;
    }

    /// <inheritdoc />
    public async ValueTask<GatewayHandlerExecutionResult> HandleAsync(
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        if (!envelope.TryGetPayload<TPayload>(out var payload))
        {
            return GatewayHandlerExecutionResult.Skipped(
                $"Payload was '{envelope.PayloadType.FullName}', expected '{typeof(TPayload).FullName}'.");
        }

        return await HandleAsync(payload, envelope, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles the typed payload.
    /// </summary>
    protected abstract ValueTask<GatewayHandlerExecutionResult> HandleAsync(
        TPayload payload,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken);
}
