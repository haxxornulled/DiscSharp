using Autofac;
using DiscSharp.Gateway.Dispatch.Orchestration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DiscSharp.Gateway.DependencyInjection;

/// <summary>
/// Registers gateway dispatch orchestration services in Autofac.
/// </summary>
public sealed class GatewayOrchestrationModule : Module
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayOrchestrationModule"/> class.
    /// </summary>
    public GatewayOrchestrationModule(IConfiguration configuration)
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
                var options = new GatewayDispatchOrchestrationOptions();
                _configuration.GetSection(GatewayDispatchOrchestrationOptions.SectionName).Bind(options);
                options.Validate();
                return Options.Create(options);
            })
            .As<IOptions<GatewayDispatchOrchestrationOptions>>()
            .SingleInstance();

        builder.RegisterType<GatewayDispatchTelemetry>()
            .SingleInstance();

        builder.RegisterType<GatewayDispatchHandlerCatalog>()
            .As<IGatewayDispatchHandlerCatalog>()
            .SingleInstance();

        builder.RegisterType<DiscordGatewayDispatchOrchestrator>()
            .As<IDiscordGatewayDispatchOrchestrator>()
            .SingleInstance();
    }
}
