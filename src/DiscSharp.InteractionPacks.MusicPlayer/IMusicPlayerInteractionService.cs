namespace DiscSharp.InteractionPacks.MusicPlayer;

/// <summary>
/// Application service port used by the Music Player interaction module.
/// </summary>
public interface IMusicPlayerInteractionService
{
    /// <summary>
    /// Plays or enqueues a track.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> PlayAsync(MusicInteractionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Pauses playback.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> PauseAsync(MusicInteractionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Resumes playback.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> ResumeAsync(MusicInteractionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Skips the current track.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> SkipAsync(MusicInteractionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Stops playback.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> StopAsync(MusicInteractionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Shows the current queue.
    /// </summary>
    ValueTask<MusicPlayerInteractionResult> QueueAsync(MusicInteractionRequest request, CancellationToken cancellationToken);
}
