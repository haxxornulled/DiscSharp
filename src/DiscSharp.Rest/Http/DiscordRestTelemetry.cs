using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DiscSharp.Rest.Http;

/// <summary>
/// Provides OpenTelemetry sources for Discord REST clients.
/// </summary>
public sealed class DiscordRestTelemetry : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestTelemetry"/> class.
    /// </summary>
    public DiscordRestTelemetry()
    {
        ActivitySource = new ActivitySource("DiscSharp.Rest");
        Meter = new Meter("DiscSharp.Rest");
        Requests = Meter.CreateCounter<long>("discsharp.rest.requests", "{request}", "Discord REST requests sent.");
        Failures = Meter.CreateCounter<long>("discsharp.rest.failures", "{request}", "Discord REST requests that failed.");
        RateLimited = Meter.CreateCounter<long>("discsharp.rest.rate_limited", "{request}", "Discord REST requests that returned HTTP 429.");
    }

    /// <summary>
    /// Gets the activity source.
    /// </summary>
    public ActivitySource ActivitySource { get; }

    /// <summary>
    /// Gets the metric meter.
    /// </summary>
    public Meter Meter { get; }

    /// <summary>
    /// Gets the request counter.
    /// </summary>
    public Counter<long> Requests { get; }

    /// <summary>
    /// Gets the failure counter.
    /// </summary>
    public Counter<long> Failures { get; }

    /// <summary>
    /// Gets the rate-limited counter.
    /// </summary>
    public Counter<long> RateLimited { get; }

    /// <summary>
    /// Disposes the telemetry sources associated with this REST client helper.
    /// </summary>
    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();
    }
}
