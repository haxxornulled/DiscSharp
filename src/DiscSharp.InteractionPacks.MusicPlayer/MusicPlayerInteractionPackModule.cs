using Autofac;
using DiscSharp.Application.Interactions;

namespace DiscSharp.InteractionPacks.MusicPlayer;

/// <summary>
/// Registers Music Player interaction pack services.
/// </summary>
public sealed class MusicPlayerInteractionPackModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.RegisterType<MusicPlayerInteractionModule>()
            .As<IDiscordInteractionModule>()
            .SingleInstance();
    }
}
