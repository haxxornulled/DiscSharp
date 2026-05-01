using DiscSharp.Application.Interactions;
using Microsoft.Extensions.Logging;

namespace DiscSharp.InteractionPacks.RaidManager;

/// <summary>
/// Handles Raid Manager Discord component and modal interactions.
/// </summary>
public sealed class RaidManagerInteractionModule : IDiscordInteractionModule
{
    private const string Module = "raid";
    private readonly IRaidManagerInteractionService _service;
    private readonly ILogger<RaidManagerInteractionModule> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RaidManagerInteractionModule"/> class.
    /// </summary>
    public RaidManagerInteractionModule(
        IRaidManagerInteractionService service,
        ILogger<RaidManagerInteractionModule> logger)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(logger);

        _service = service;
        _logger = logger;
    }

    /// <inheritdoc />
    public string ModuleName => Module;

    /// <inheritdoc />
    public int Order => 1_000;

    /// <inheritdoc />
    public bool CanHandle(DiscordInteractionEnvelope interaction)
    {
        ArgumentNullException.ThrowIfNull(interaction);

        if (interaction.Kind is not (DiscordInteractionKind.MessageComponent or DiscordInteractionKind.ModalSubmit))
        {
            return false;
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

        if (!DiscordComponentCustomId.TryParse(interaction.CustomId, out var customId, out var error))
        {
            return InteractionModuleResult.FailedWith(error ?? "Invalid raid interaction custom ID.");
        }

        if (!customId.Arguments.TryGetValue("raidId", out var raidId) || string.IsNullOrWhiteSpace(raidId))
        {
            return InteractionModuleResult.HandledWith(
                InteractionResponsePlan.Message("Raid interaction is missing a raid identifier.", ephemeral: true),
                "Missing raidId.");
        }

        var request = new RaidInteractionRequest(
            interaction.InteractionId,
            interaction.GuildId,
            interaction.ChannelId,
            interaction.UserId,
            raidId,
            customId.Arguments.GetValueOrDefault("role"));

        var result = customId.Action switch
        {
            "join" => await _service.JoinAsync(request, cancellationToken).ConfigureAwait(false),
            "leave" => await _service.LeaveAsync(request, cancellationToken).ConfigureAwait(false),
            "start" => await _service.StartAsync(request, cancellationToken).ConfigureAwait(false),
            "cancel" => await _service.CancelAsync(request, cancellationToken).ConfigureAwait(false),
            _ => RaidManagerInteractionResult.Fail($"Unsupported raid action '{customId.Action}'.")
        };

        _logger.LogInformation(
            "Raid interaction {InteractionId} action {Action} raid {RaidId} success={Success}",
            interaction.InteractionId,
            customId.Action,
            raidId,
            result.Success);

        return InteractionModuleResult.HandledWith(
            InteractionResponsePlan.Message(result.Message, result.Ephemeral),
            result.Success ? "Raid interaction handled." : "Raid interaction failed.");
    }
}
