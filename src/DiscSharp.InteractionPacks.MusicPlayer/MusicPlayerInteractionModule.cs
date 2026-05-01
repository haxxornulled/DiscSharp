using DiscSharp.Application.Interactions;
using Microsoft.Extensions.Logging;

namespace DiscSharp.InteractionPacks.MusicPlayer;

/// <summary>
/// Handles Music Player Discord component, modal, and command interactions.
/// </summary>
public sealed class MusicPlayerInteractionModule : IDiscordInteractionModule
{
    private const string Module = "music";
    private readonly IMusicPlayerInteractionService _service;
    private readonly ILogger<MusicPlayerInteractionModule> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicPlayerInteractionModule"/> class.
    /// </summary>
    public MusicPlayerInteractionModule(
        IMusicPlayerInteractionService service,
        ILogger<MusicPlayerInteractionModule> logger)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(logger);

        _service = service;
        _logger = logger;
    }

    /// <inheritdoc />
    public string ModuleName => Module;

    /// <inheritdoc />
    public int Order => 1_100;

    /// <inheritdoc />
    public bool CanHandle(DiscordInteractionEnvelope interaction)
    {
        ArgumentNullException.ThrowIfNull(interaction);

        if (interaction.Kind == DiscordInteractionKind.ApplicationCommand)
        {
            return string.Equals(interaction.CommandName, "music", StringComparison.OrdinalIgnoreCase);
        }

        return DiscordComponentCustomId.TryParse(interaction.CustomId, out var customId, out _)
            && string.Equals(customId.Module, Module, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public async ValueTask<InteractionModuleResult> HandleAsync(
        DiscordInteractionEnvelope interaction,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(interaction);

        var action = ResolveAction(interaction, out var arguments);
        if (string.IsNullOrWhiteSpace(action))
        {
            return InteractionModuleResult.HandledWith(
                InteractionResponsePlan.Message("Music interaction did not include a playable action.", ephemeral: true),
                "Missing music action.");
        }

        var request = new MusicInteractionRequest(
            interaction.InteractionId,
            interaction.GuildId,
            interaction.ChannelId,
            interaction.UserId,
            arguments.GetValueOrDefault("query"),
            arguments);

        var result = action switch
        {
            "play" => await _service.PlayAsync(request, cancellationToken).ConfigureAwait(false),
            "pause" => await _service.PauseAsync(request, cancellationToken).ConfigureAwait(false),
            "resume" => await _service.ResumeAsync(request, cancellationToken).ConfigureAwait(false),
            "skip" => await _service.SkipAsync(request, cancellationToken).ConfigureAwait(false),
            "stop" => await _service.StopAsync(request, cancellationToken).ConfigureAwait(false),
            "queue" => await _service.QueueAsync(request, cancellationToken).ConfigureAwait(false),
            _ => MusicPlayerInteractionResult.Fail($"Unsupported music action '{action}'.")
        };

        _logger.LogInformation(
            "Music interaction {InteractionId} action {Action} success={Success}",
            interaction.InteractionId,
            action,
            result.Success);

        return InteractionModuleResult.HandledWith(
            InteractionResponsePlan.Message(result.Message, result.Ephemeral),
            result.Success ? "Music interaction handled." : "Music interaction failed.");
    }

    private static string? ResolveAction(
        DiscordInteractionEnvelope interaction,
        out IReadOnlyDictionary<string, string> arguments)
    {
        if (DiscordComponentCustomId.TryParse(interaction.CustomId, out var customId, out _)
            && string.Equals(customId.Module, Module, StringComparison.Ordinal))
        {
            arguments = customId.Arguments;
            return customId.Action;
        }

        arguments = new Dictionary<string, string>(StringComparer.Ordinal);
        return interaction.Kind == DiscordInteractionKind.ApplicationCommand
            ? "queue"
            : null;
    }
}
