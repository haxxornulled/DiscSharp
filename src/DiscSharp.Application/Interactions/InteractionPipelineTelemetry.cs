using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DiscSharp.Application.Interactions;

/// <summary>
/// Owns interaction pipeline activity and metric instruments.
/// </summary>
public sealed class InteractionPipelineTelemetry
{
    /// <summary>
    /// The interaction pipeline activity source name.
    /// </summary>
    public const string ActivitySourceName = "DiscSharp.Interactions.Pipeline";

    /// <summary>
    /// The interaction pipeline meter name.
    /// </summary>
    public const string MeterName = "DiscSharp.Interactions.Pipeline";

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionPipelineTelemetry"/> class.
    /// </summary>
    public InteractionPipelineTelemetry()
    {
        ActivitySource = new ActivitySource(ActivitySourceName);
        Meter = new Meter(MeterName);
        Executions = Meter.CreateCounter<long>("discsharp.interactions.pipeline.executions");
        Failures = Meter.CreateCounter<long>("discsharp.interactions.pipeline.failures");
        Duration = Meter.CreateHistogram<double>("discsharp.interactions.pipeline.duration_ms", unit: "ms");
    }

    /// <summary>
    /// Gets the activity source.
    /// </summary>
    public ActivitySource ActivitySource { get; }

    /// <summary>
    /// Gets the meter.
    /// </summary>
    public Meter Meter { get; }

    /// <summary>
    /// Gets the pipeline execution counter.
    /// </summary>
    public Counter<long> Executions { get; }

    /// <summary>
    /// Gets the pipeline failure counter.
    /// </summary>
    public Counter<long> Failures { get; }

    /// <summary>
    /// Gets the pipeline duration histogram.
    /// </summary>
    public Histogram<double> Duration { get; }
}
