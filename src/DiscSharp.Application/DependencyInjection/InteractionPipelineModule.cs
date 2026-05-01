using Autofac;
using DiscSharp.Application.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DiscSharp.Application.DependencyInjection;

/// <summary>
/// Registers interaction pipeline services in Autofac.
/// </summary>
public sealed class InteractionPipelineModule : Module
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionPipelineModule"/> class.
    /// </summary>
    public InteractionPipelineModule(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Register(_ =>
            {
                var options = new InteractionPipelineOptions();
                _configuration.GetSection(InteractionPipelineOptions.SectionName).Bind(options);
                return Options.Create(options);
            })
            .As<IOptions<InteractionPipelineOptions>>()
            .SingleInstance();

        builder.RegisterType<InteractionPipelineTelemetry>()
            .SingleInstance();

        builder.RegisterType<DiscordInteractionPipeline>()
            .As<IDiscordInteractionPipeline>()
            .SingleInstance();
    }
}
