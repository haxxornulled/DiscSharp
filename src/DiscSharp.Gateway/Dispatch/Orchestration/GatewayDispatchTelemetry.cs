using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Owns gateway dispatch activity and metric instruments.
/// </summary>
public sealed class GatewayDispatchTelemetry
{
    /// <summary>
    /// The gateway dispatch activity source name.
    /// </summary>
    public const string ActivitySourceName = "DiscSharp.Gateway.Dispatch";

    /// <summary>
    /// The gateway dispatch meter name.
    /// </summary>
    public const string MeterName = "DiscSharp.Gateway.Dispatch";

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayDispatchTelemetry"/> class.
    /// </summary>
    public GatewayDispatchTelemetry()
    {
        ActivitySource = new ActivitySource(ActivitySourceName);
        Meter = new Meter(MeterName);
        DispatchCounter = Meter.CreateCounter<long>("discsharp.gateway.dispatch.events");
        HandlerCounter = Meter.CreateCounter<long>("discsharp.gateway.dispatch.handlers");
        HandlerFailureCounter = Meter.CreateCounter<long>("discsharp.gateway.dispatch.handler_failures");
        DispatchDuration = Meter.CreateHistogram<double>("discsharp.gateway.dispatch.duration_ms", unit: "ms");
        HandlerDuration = Meter.CreateHistogram<double>("discsharp.gateway.dispatch.handler_duration_ms", unit: "ms");
    }

    /// <summary>
    /// Gets the gateway dispatch activity source.
    /// </summary>
    public ActivitySource ActivitySource { get; }

    /// <summary>
    /// Gets the gateway dispatch meter.
    /// </summary>
    public Meter Meter { get; }

    /// <summary>
    /// Gets the dispatch counter.
    /// </summary>
    public Counter<long> DispatchCounter { get; }

    /// <summary>
    /// Gets the handler invocation counter.
    /// </summary>
    public Counter<long> HandlerCounter { get; }

    /// <summary>
    /// Gets the handler failure counter.
    /// </summary>
    public Counter<long> HandlerFailureCounter { get; }

    /// <summary>
    /// Gets the dispatch duration histogram.
    /// </summary>
    public Histogram<double> DispatchDuration { get; }

    /// <summary>
    /// Gets the handler duration histogram.
    /// </summary>
    public Histogram<double> HandlerDuration { get; }
}
