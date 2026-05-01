using Autofac;
using DiscSharp.Application.Interactions;

namespace DiscSharp.InteractionPacks.RaidManager;

/// <summary>
/// Registers Raid Manager interaction pack services.
/// </summary>
public sealed class RaidManagerInteractionPackModule : Module
{
    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.RegisterType<RaidManagerInteractionModule>()
            .As<IDiscordInteractionModule>()
            .SingleInstance();
    }
}
