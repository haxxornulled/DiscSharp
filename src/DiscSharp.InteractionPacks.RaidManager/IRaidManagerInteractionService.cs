namespace DiscSharp.InteractionPacks.RaidManager;

/// <summary>
/// Application service port used by the Raid Manager interaction module.
/// </summary>
public interface IRaidManagerInteractionService
{
    /// <summary>
    /// Joins the specified raid.
    /// </summary>
    ValueTask<RaidManagerInteractionResult> JoinAsync(
        RaidInteractionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Leaves the specified raid.
    /// </summary>
    ValueTask<RaidManagerInteractionResult> LeaveAsync(
        RaidInteractionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Starts the specified raid.
    /// </summary>
    ValueTask<RaidManagerInteractionResult> StartAsync(
        RaidInteractionRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cancels the specified raid.
    /// </summary>
    ValueTask<RaidManagerInteractionResult> CancelAsync(
        RaidInteractionRequest request,
        CancellationToken cancellationToken);
}
