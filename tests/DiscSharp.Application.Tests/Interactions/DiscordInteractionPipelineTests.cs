using DiscSharp.Application.Interactions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DiscSharp.Application.Tests.Interactions;

public sealed class DiscordInteractionPipelineTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Invoke_First_Module_That_Handles_Interaction()
    {
        var writer = new CapturingResponseWriter();
        var pipeline = new DiscordInteractionPipeline(
            new IDiscordInteractionModule[]
            {
                new TestModule("skip", canHandle: false),
                new TestModule("hit", canHandle: true)
            },
            writer,
            Options.Create(new InteractionPipelineOptions()),
            new InteractionPipelineTelemetry(),
            NullLogger<DiscordInteractionPipeline>.Instance);

        var interaction = CreateInteraction("raid/join?raidId=123");
        var result = await pipeline.ExecuteAsync(interaction, CancellationToken.None);

        Assert.True(result.Handled);
        Assert.Equal("hit", result.ModuleName);
        Assert.Equal("handled", writer.LastPlan?.Content);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Write_Unhandled_Response_When_No_Module_Matches()
    {
        var writer = new CapturingResponseWriter();
        var pipeline = new DiscordInteractionPipeline(
            new[] { new TestModule("skip", canHandle: false) },
            writer,
            Options.Create(new InteractionPipelineOptions { UnhandledResponseContent = "nope" }),
            new InteractionPipelineTelemetry(),
            NullLogger<DiscordInteractionPipeline>.Instance);

        var interaction = CreateInteraction("raid/join?raidId=123");
        var result = await pipeline.ExecuteAsync(interaction, CancellationToken.None);

        Assert.False(result.Handled);
        Assert.Equal("nope", writer.LastPlan?.Content);
    }

    private static DiscordInteractionEnvelope CreateInteraction(string customId) =>
        new(
            interactionId: "1",
            applicationId: "2",
            DiscordInteractionKind.MessageComponent,
            token: "token",
            guildId: "guild",
            channelId: "channel",
            userId: "user",
            commandName: null,
            customId,
            rawPayload: new object(),
            DateTimeOffset.UtcNow);

    private sealed class CapturingResponseWriter : IInteractionResponseWriter
    {
        public InteractionResponsePlan? LastPlan { get; private set; }

        public ValueTask WriteAsync(
            DiscordInteractionEnvelope interaction,
            InteractionResponsePlan responsePlan,
            CancellationToken cancellationToken)
        {
            LastPlan = responsePlan;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class TestModule : IDiscordInteractionModule
    {
        private readonly bool _canHandle;

        public TestModule(string moduleName, bool canHandle)
        {
            ModuleName = moduleName;
            _canHandle = canHandle;
        }

        public string ModuleName { get; }

        public int Order => 0;

        public bool CanHandle(DiscordInteractionEnvelope interaction) => _canHandle;

        public ValueTask<InteractionModuleResult> HandleAsync(
            DiscordInteractionEnvelope interaction,
            CancellationToken cancellationToken) =>
            ValueTask.FromResult(InteractionModuleResult.HandledWith(InteractionResponsePlan.Message("handled")));
    }
}
