using Autofac;
using DiscSharp.Rest.Http;
using DiscSharp.Rest.Interactions;
using Microsoft.Extensions.Options;

namespace DiscSharp.Rest.DependencyInjection;

/// <summary>
/// Autofac module for Discord REST API services.
/// </summary>
public sealed class DiscordRestModule : Module
{
    private readonly Action<DiscordApiOptions>? _configure;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscordRestModule"/> class.
    /// </summary>
    public DiscordRestModule(Action<DiscordApiOptions>? configure = null)
    {
        _configure = configure;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Register(_ =>
            {
                var options = new DiscordApiOptions();
                _configure?.Invoke(options);
                options.Validate();
                return options;
            })
            .AsSelf()
            .SingleInstance();

        builder.Register(static ctx => Options.Create(ctx.Resolve<DiscordApiOptions>()))
            .As<IOptions<DiscordApiOptions>>()
            .SingleInstance();

        builder.Register(static ctx =>
            {
                var options = ctx.Resolve<DiscordApiOptions>();
                return new HttpClient { BaseAddress = options.VersionedBaseUri };
            })
            .As<HttpClient>()
            .SingleInstance();

        builder.RegisterType<DiscordRestTelemetry>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<DiscordInteractionRestClient>()
            .As<IDiscordInteractionRestClient>()
            .SingleInstance();
    }
}
