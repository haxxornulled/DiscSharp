using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscSharp.Application.Interactions;

/// <summary>
/// Default implementation of the Discord interaction pipeline.
/// </summary>
public sealed class DiscordInteractionPipeline : IDiscordInteractionPipeline
{
    private readonly IReadOnlyList<IDiscordInteractionModule> _modules;
    private readonly IInteractionResponseWriter _responseWriter;
    private readonly InteractionPipelineOptions _options;
    private readonly InteractionPipelineTelemetry _telemetry;
    private readonly ILogger<DiscordInteractionPipeline> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordInteractionPipeline"/> class.
    /// </summary>
    public DiscordInteractionPipeline(
        IEnumerable<IDiscordInteractionModule> modules,
        IInteractionResponseWriter responseWriter,
        IOptions<InteractionPipelineOptions> options,
        InteractionPipelineTelemetry telemetry,
        ILogger<DiscordInteractionPipeline> logger)
    {
        ArgumentNullException.ThrowIfNull(modules);
        ArgumentNullException.ThrowIfNull(responseWriter);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(telemetry);
        ArgumentNullException.ThrowIfNull(logger);

        _modules = modules
            .OrderBy(static module => module.Order)
            .ThenBy(static module => module.ModuleName, StringComparer.Ordinal)
            .ToArray();
        _responseWriter = responseWriter;
        _options = options.Value;
        _telemetry = telemetry;
        _logger = logger;
    }

    /// <summary>
    /// Executes the interaction pipeline until a module handles the interaction, the pipeline writes a fallback response,
    /// or a module failure is converted into a failure response.
    /// </summary>
    public async ValueTask<DiscordInteractionPipelineResult> ExecuteAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(interaction);

        using var activity = _telemetry.ActivitySource.StartActivity("Discord interaction pipeline", ActivityKind.Internal);
        activity?.SetTag("discord.interaction.id", interaction.InteractionId);
        activity?.SetTag("discord.interaction.kind", interaction.Kind.ToString());
        activity?.SetTag("discord.interaction.guild_id", interaction.GuildId);
        activity?.SetTag("discord.interaction.channel_id", interaction.ChannelId);
        activity?.SetTag("discord.interaction.command_name", interaction.CommandName);

        var started = Stopwatch.GetTimestamp();
        var tags = new TagList
        {
            { "kind", interaction.Kind.ToString() },
            { "guild_id", interaction.GuildId ?? "dm" }
        };
        _telemetry.Executions.Add(1, tags);

        foreach (var module in _modules)
        {
            if (!module.CanHandle(interaction))
            {
                continue;
            }

            try
            {
                var result = await module.HandleAsync(interaction, cancellationToken).ConfigureAwait(false);
                if (!result.Handled)
                {
                    continue;
                }

                await _responseWriter.WriteAsync(interaction, result.ResponsePlan, cancellationToken).ConfigureAwait(false);
                var handledDuration = Stopwatch.GetElapsedTime(started);
                _telemetry.Duration.Record(handledDuration.TotalMilliseconds, tags);

                _logger.LogInformation(
                    "Interaction {InteractionId} handled by {ModuleName} with response {ResponseKind} in {ElapsedMs}ms",
                    interaction.InteractionId,
                    module.ModuleName,
                    result.ResponsePlan.Kind,
                    handledDuration.TotalMilliseconds);

                return new DiscordInteractionPipelineResult(
                    interaction.InteractionId,
                    interaction.Kind,
                    Handled: true,
                    module.ModuleName,
                    result.ResponsePlan,
                    handledDuration,
                    result.Message,
                    result.Exception);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _telemetry.Failures.Add(1, tags);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                _logger.LogError(
                    ex,
                    "Interaction module {ModuleName} threw while handling interaction {InteractionId}",
                    module.ModuleName,
                    interaction.InteractionId);

                var failurePlan = _options.RespondToModuleFailures
                    ? InteractionResponsePlan.Message(_options.FailureResponseContent, ephemeral: true)
                    : InteractionResponsePlan.None();

                await _responseWriter.WriteAsync(interaction, failurePlan, cancellationToken).ConfigureAwait(false);
                var failedDuration = Stopwatch.GetElapsedTime(started);
                _telemetry.Duration.Record(failedDuration.TotalMilliseconds, tags);

                return new DiscordInteractionPipelineResult(
                    interaction.InteractionId,
                    interaction.Kind,
                    Handled: false,
                    module.ModuleName,
                    failurePlan,
                    failedDuration,
                    ex.Message,
                    ex);
            }
        }

        var unhandledPlan = _options.RespondToUnhandledInteractions
            ? InteractionResponsePlan.Message(_options.UnhandledResponseContent, ephemeral: true)
            : InteractionResponsePlan.None();

        await _responseWriter.WriteAsync(interaction, unhandledPlan, cancellationToken).ConfigureAwait(false);
        var duration = Stopwatch.GetElapsedTime(started);
        _telemetry.Duration.Record(duration.TotalMilliseconds, tags);

        _logger.LogDebug(
            "Interaction {InteractionId} kind {InteractionKind} was not handled in {ElapsedMs}ms",
            interaction.InteractionId,
            interaction.Kind,
            duration.TotalMilliseconds);

        return new DiscordInteractionPipelineResult(
            interaction.InteractionId,
            interaction.Kind,
            Handled: false,
            ModuleName: null,
            unhandledPlan,
            duration,
            "Interaction was not handled.",
            Exception: null);
    }
}
