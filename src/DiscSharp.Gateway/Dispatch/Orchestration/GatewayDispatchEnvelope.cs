using System.Diagnostics.CodeAnalysis;

namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Represents a typed Discord gateway dispatch event after the gateway payload has been decoded.
/// </summary>
public sealed record GatewayDispatchEnvelope
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayDispatchEnvelope"/> class.
    /// </summary>
    public GatewayDispatchEnvelope(
        string eventName,
        object payload,
        Type payloadType,
        long? sequenceNumber,
        DateTimeOffset receivedAt,
        string? shardId = null,
        string? correlationId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventName);
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(payloadType);

        if (!payloadType.IsInstanceOfType(payload))
        {
            throw new ArgumentException(
                $"Payload instance type '{payload.GetType().FullName}' is not assignable to declared payload type '{payloadType.FullName}'.",
                nameof(payloadType));
        }

        EventName = eventName;
        Payload = payload;
        PayloadType = payloadType;
        SequenceNumber = sequenceNumber;
        ReceivedAt = receivedAt;
        ShardId = shardId;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets the Discord dispatch event name, such as <c>INTERACTION_CREATE</c>.
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// Gets the typed dispatch payload instance.
    /// </summary>
    public object Payload { get; }

    /// <summary>
    /// Gets the declared payload type.
    /// </summary>
    public Type PayloadType { get; }

    /// <summary>
    /// Gets the gateway sequence number, when Discord provided one.
    /// </summary>
    public long? SequenceNumber { get; }

    /// <summary>
    /// Gets the timestamp at which the event entered the local gateway pipeline.
    /// </summary>
    public DateTimeOffset ReceivedAt { get; }

    /// <summary>
    /// Gets the shard identifier associated with this dispatch, when available.
    /// </summary>
    public string? ShardId { get; }

    /// <summary>
    /// Gets the correlation identifier associated with this dispatch, when available.
    /// </summary>
    public string? CorrelationId { get; }

    /// <summary>
    /// Attempts to read the payload as <typeparamref name="TPayload"/>.
    /// </summary>
    public bool TryGetPayload<TPayload>([NotNullWhen(true)] out TPayload? payload)
        where TPayload : class
    {
        if (Payload is TPayload typed)
        {
            payload = typed;
            return true;
        }

        payload = null;
        return false;
    }

    /// <summary>
    /// Reads the payload as <typeparamref name="TPayload"/> or throws when the envelope contains a different type.
    /// </summary>
    public TPayload GetPayload<TPayload>()
        where TPayload : class
    {
        if (TryGetPayload<TPayload>(out var payload))
        {
            return payload;
        }

        throw new InvalidOperationException(
            $"Dispatch payload for '{EventName}' is '{PayloadType.FullName}', not '{typeof(TPayload).FullName}'.");
    }
}
