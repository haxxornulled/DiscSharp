using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Default multi-handler gateway dispatch orchestrator.
/// </summary>
public sealed class DiscordGatewayDispatchOrchestrator : IDiscordGatewayDispatchOrchestrator
{
    private readonly IGatewayDispatchHandlerCatalog _catalog;
    private readonly GatewayDispatchOrchestrationOptions _options;
    private readonly GatewayDispatchTelemetry _telemetry;
    private readonly ILogger<DiscordGatewayDispatchOrchestrator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordGatewayDispatchOrchestrator"/> class.
    /// </summary>
    public DiscordGatewayDispatchOrchestrator(
        IGatewayDispatchHandlerCatalog catalog,
        IOptions<GatewayDispatchOrchestrationOptions> options,
        GatewayDispatchTelemetry telemetry,
        ILogger<DiscordGatewayDispatchOrchestrator> logger)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(telemetry);
        ArgumentNullException.ThrowIfNull(logger);

        _catalog = catalog;
        _options = options.Value;
        _options.Validate();
        _telemetry = telemetry;
        _logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<GatewayDispatchOrchestrationResult> DispatchAsync(
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        using var activity = _telemetry.ActivitySource.StartActivity("Gateway dispatch", ActivityKind.Internal);
        activity?.SetTag("discord.gateway.event_name", envelope.EventName);
        activity?.SetTag("discord.gateway.payload_type", envelope.PayloadType.FullName);
        activity?.SetTag("discord.gateway.sequence_number", envelope.SequenceNumber);
        activity?.SetTag("discord.gateway.shard_id", envelope.ShardId);
        activity?.SetTag("discsharp.correlation_id", envelope.CorrelationId);

        var started = Stopwatch.GetTimestamp();
        var handlers = _catalog.FindHandlers(envelope);
        var tags = CreateDispatchTags(envelope);
        _telemetry.DispatchCounter.Add(1, tags);

        if (handlers.Count == 0)
        {
            if (_options.LogUnhandledDispatches)
            {
                _logger.LogDebug(
                    "No gateway handlers matched {EventName} payload {PayloadType}",
                    envelope.EventName,
                    envelope.PayloadType.FullName);
            }

            var unhandledDuration = Stopwatch.GetElapsedTime(started);
            _telemetry.DispatchDuration.Record(unhandledDuration.TotalMilliseconds, tags);
            return new GatewayDispatchOrchestrationResult(
                envelope.EventName,
                envelope.PayloadType,
                envelope.SequenceNumber,
                MatchedHandlerCount: 0,
                HandlerResults: Array.Empty<GatewayHandlerInvocationResult>(),
                unhandledDuration);
        }

        _logger.LogTrace(
            "Dispatching {EventName} to {HandlerCount} handlers",
            envelope.EventName,
            handlers.Count);

        var results = _options.ExecutionMode switch
        {
            GatewayHandlerExecutionMode.Sequential => await ExecuteSequentialAsync(handlers, envelope, cancellationToken).ConfigureAwait(false),
            GatewayHandlerExecutionMode.ParallelByOrderBand => await ExecuteParallelByOrderBandAsync(handlers, envelope, cancellationToken).ConfigureAwait(false),
            _ => throw new InvalidOperationException($"Unsupported gateway handler execution mode '{_options.ExecutionMode}'.")
        };

        var duration = Stopwatch.GetElapsedTime(started);
        _telemetry.DispatchDuration.Record(duration.TotalMilliseconds, tags);

        if (results.Any(static result => result.IsFailure))
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            _logger.LogWarning(
                "Gateway dispatch {EventName} completed with {FailureCount} handler failures in {ElapsedMs}ms",
                envelope.EventName,
                results.Count(static result => result.IsFailure),
                duration.TotalMilliseconds);
        }
        else
        {
            _logger.LogTrace(
                "Gateway dispatch {EventName} completed successfully in {ElapsedMs}ms",
                envelope.EventName,
                duration.TotalMilliseconds);
        }

        return new GatewayDispatchOrchestrationResult(
            envelope.EventName,
            envelope.PayloadType,
            envelope.SequenceNumber,
            handlers.Count,
            results,
            duration);
    }

    private async ValueTask<IReadOnlyList<GatewayHandlerInvocationResult>> ExecuteSequentialAsync(
        IReadOnlyList<IDiscordGatewayDispatchHandler> handlers,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        var results = new List<GatewayHandlerInvocationResult>(handlers.Count);

        foreach (var handler in handlers)
        {
            var result = await InvokeHandlerAsync(handler, envelope, cancellationToken).ConfigureAwait(false);
            results.Add(result);

            if (result.IsFailure && handler.FailurePolicy == GatewayHandlerFailurePolicy.StopPipeline)
            {
                _logger.LogWarning(
                    "Stopping gateway dispatch pipeline for {EventName} after handler {HandlerName} failed with StopPipeline policy",
                    envelope.EventName,
                    handler.HandlerName);
                break;
            }
        }

        return results;
    }

    private async ValueTask<IReadOnlyList<GatewayHandlerInvocationResult>> ExecuteParallelByOrderBandAsync(
        IReadOnlyList<IDiscordGatewayDispatchHandler> handlers,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        var results = new List<GatewayHandlerInvocationResult>(handlers.Count);
        foreach (var orderBand in handlers.GroupBy(static handler => handler.Order).OrderBy(static group => group.Key))
        {
            var bandHandlers = orderBand.ToArray();
            if (bandHandlers.Length > _options.MaxParallelHandlersPerOrderBand)
            {
                throw new InvalidOperationException(
                    $"Gateway handler order band {orderBand.Key} has {bandHandlers.Length} handlers, exceeding configured maximum {_options.MaxParallelHandlersPerOrderBand}.");
            }

            var bandResults = await Task.WhenAll(
                bandHandlers.Select(handler => InvokeHandlerAsTaskAsync(handler, envelope, cancellationToken))).ConfigureAwait(false);

            results.AddRange(bandResults.OrderBy(static result => result.Order).ThenBy(static result => result.HandlerName, StringComparer.Ordinal));

            var stopFailure = bandResults.FirstOrDefault(result =>
                result.IsFailure
                && bandHandlers.Any(handler => string.Equals(handler.HandlerName, result.HandlerName, StringComparison.Ordinal)
                    && handler.FailurePolicy == GatewayHandlerFailurePolicy.StopPipeline));

            if (stopFailure is not null)
            {
                _logger.LogWarning(
                    "Stopping gateway dispatch pipeline for {EventName} after order band {Order} failed with StopPipeline policy",
                    envelope.EventName,
                    orderBand.Key);
                break;
            }
        }

        return results;
    }

    private async Task<GatewayHandlerInvocationResult> InvokeHandlerAsTaskAsync(
        IDiscordGatewayDispatchHandler handler,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken) =>
        await InvokeHandlerAsync(handler, envelope, cancellationToken).ConfigureAwait(false);

    private async ValueTask<GatewayHandlerInvocationResult> InvokeHandlerAsync(
        IDiscordGatewayDispatchHandler handler,
        GatewayDispatchEnvelope envelope,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(envelope);

        using var activity = _telemetry.ActivitySource.StartActivity("Gateway handler", ActivityKind.Internal);
        activity?.SetTag("discord.gateway.event_name", envelope.EventName);
        activity?.SetTag("discord.gateway.handler", handler.HandlerName);
        activity?.SetTag("discord.gateway.handler_order", handler.Order);
        activity?.SetTag("discord.gateway.payload_type", envelope.PayloadType.FullName);

        var started = Stopwatch.GetTimestamp();
        var tags = CreateHandlerTags(envelope, handler);
        _telemetry.HandlerCounter.Add(1, tags);

        try
        {
            using var timeoutCts = CreateTimeoutCancellationTokenSource(cancellationToken);
            var effectiveToken = timeoutCts?.Token ?? cancellationToken;

            var executionResult = await handler.HandleAsync(envelope, effectiveToken).ConfigureAwait(false);
            var duration = Stopwatch.GetElapsedTime(started);
            _telemetry.HandlerDuration.Record(duration.TotalMilliseconds, tags);

            if (executionResult.Status == GatewayHandlerExecutionStatus.Failed)
            {
                _telemetry.HandlerFailureCounter.Add(1, tags);
                activity?.SetStatus(ActivityStatusCode.Error, executionResult.Message);

                _logger.LogWarning(
                    executionResult.Exception,
                    "Gateway handler {HandlerName} failed for {EventName}: {Message}",
                    handler.HandlerName,
                    envelope.EventName,
                    executionResult.Message);
            }

            return new GatewayHandlerInvocationResult(
                handler.HandlerName,
                envelope.EventName,
                handler.PayloadType,
                handler.Order,
                executionResult.Status,
                duration,
                executionResult.Message,
                executionResult.Exception);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            var duration = Stopwatch.GetElapsedTime(started);
            _telemetry.HandlerDuration.Record(duration.TotalMilliseconds, tags);
            _telemetry.HandlerFailureCounter.Add(1, tags);
            activity?.SetStatus(ActivityStatusCode.Error, "Gateway handler timed out.");

            _logger.LogWarning(
                ex,
                "Gateway handler {HandlerName} timed out after {ElapsedMs}ms for {EventName}",
                handler.HandlerName,
                duration.TotalMilliseconds,
                envelope.EventName);

            return new GatewayHandlerInvocationResult(
                handler.HandlerName,
                envelope.EventName,
                handler.PayloadType,
                handler.Order,
                GatewayHandlerExecutionStatus.Failed,
                duration,
                "Gateway handler timed out.",
                ex);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var duration = Stopwatch.GetElapsedTime(started);
            _telemetry.HandlerDuration.Record(duration.TotalMilliseconds, tags);
            _telemetry.HandlerFailureCounter.Add(1, tags);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            _logger.LogError(
                ex,
                "Gateway handler {HandlerName} threw while handling {EventName}",
                handler.HandlerName,
                envelope.EventName);

            return new GatewayHandlerInvocationResult(
                handler.HandlerName,
                envelope.EventName,
                handler.PayloadType,
                handler.Order,
                GatewayHandlerExecutionStatus.Failed,
                duration,
                ex.Message,
                ex);
        }
    }

    private CancellationTokenSource? CreateTimeoutCancellationTokenSource(CancellationToken cancellationToken)
    {
        if (_options.HandlerTimeout is not { } timeout)
        {
            return null;
        }

        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);
        return timeoutCts;
    }

    private static TagList CreateDispatchTags(GatewayDispatchEnvelope envelope)
    {
        var tags = new TagList
        {
            { "event_name", envelope.EventName },
            { "payload_type", envelope.PayloadType.Name }
        };

        if (!string.IsNullOrWhiteSpace(envelope.ShardId))
        {
            tags.Add("shard_id", envelope.ShardId);
        }

        return tags;
    }

    private static TagList CreateHandlerTags(GatewayDispatchEnvelope envelope, IDiscordGatewayDispatchHandler handler)
    {
        var tags = CreateDispatchTags(envelope);
        tags.Add("handler", handler.HandlerName);
        tags.Add("handler_order", handler.Order);
        return tags;
    }
}
