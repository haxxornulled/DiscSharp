using DiscSharp.Application.Interactions;
using DiscSharp.Gateway.Dispatch.Orchestration;
using Microsoft.Extensions.Logging;

namespace DiscSharp.Gateway.Interactions;

/// <summary>
/// Bridges an existing typed <c>INTERACTION_CREATE</c> gateway event into the interaction pipeline.
/// </summary>
/// <typeparam name="TInteractionCreateEvent">The existing typed interaction-create dispatch DTO.</typeparam>
public sealed class InteractionCreateGatewayDispatchHandler<TInteractionCreateEvent> : DiscordGatewayDispatchHandler<TInteractionCreateEvent>
    where TInteractionCreateEvent : class
{
    private readonly IInteractionEnvelopeFactory<TInteractionCreateEvent> _interactionFactory;
    private readonly IDiscordInteractionPipeline _interactionPipeline;
    private readonly ILogger<InteractionCreateGatewayDispatchHandler<TInteractionCreateEvent>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionCreateGatewayDispatchHandler{TInteractionCreateEvent}"/> class.
    /// </summary>
    public InteractionCreateGatewayDispatchHandler(
        IInteractionEnvelopeFactory<TInteractionCreateEvent> interactionFactory,
        IDiscordInteractionPipeline interactionPipeline,
        ILogger<InteractionCreateGatewayDispatchHandler<TInteractionCreateEvent>> logger)
    {
        ArgumentNullException.ThrowIfNull(interactionFactory);
        ArgumentNullException.ThrowIfNull(interactionPipeline);
        ArgumentNullException.ThrowIfNull(logger);

        _interactionFactory = interactionFactory;
        _interactionPipeline = interactionPipeline;
        _logger = logger;
    }

    /// <inheritdoc />
    public override string EventName => "INTERACTION_CREATE";

    /// <inheritdoc />
    public override int Order => 100;

    /// <inheritdoc />
    public override GatewayHandlerFailurePolicy FailurePolicy => GatewayHandlerFailurePolicy.Continue;

    /// <inheritdoc />
    protected override async ValueTask<GatewayHandlerExecutionResult> HandleAsync(
        TInteractionCreateEvent payload,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(envelope);

        var interaction = _interactionFactory.Create(payload, envelope.ReceivedAt);
        var result = await _interactionPipeline.ExecuteAsync(interaction, cancellationToken).ConfigureAwait(false);

        if (result.Failed)
        {
            return GatewayHandlerExecutionResult.Failed(
                $"Interaction pipeline failed for interaction '{interaction.InteractionId}'.",
                result.Exception);
        }

        _logger.LogDebug(
            "Interaction {InteractionId} handled={Handled} module={ModuleName}",
            interaction.InteractionId,
            result.Handled,
            result.ModuleName);

        return GatewayHandlerExecutionResult.Succeeded(
            result.Handled
                ? $"Interaction handled by '{result.ModuleName}'."
                : "Interaction was not handled.");
    }
}
