using DiscSharp.Gateway.Dispatch.Orchestration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DiscSharp.Gateway.Tests.Dispatch.Orchestration;

public sealed class GatewayDispatchOrchestratorTests
{
    [Fact]
    public async Task DispatchAsync_Should_Invoke_Matching_Handlers_In_Order()
    {
        var calls = new List<string>();
        var handlers = new IDiscordGatewayDispatchHandler[]
        {
            new TestHandler("second", order: 20, calls),
            new TestHandler("first", order: 10, calls)
        };
        var orchestrator = CreateOrchestrator(handlers);
        var envelope = CreateEnvelope(new TestPayload("hello"));

        var result = await orchestrator.DispatchAsync(envelope, CancellationToken.None);

        Assert.False(result.HasFailures);
        Assert.Equal(2, result.MatchedHandlerCount);
        Assert.Equal(new[] { "first", "second" }, calls);
    }

    [Fact]
    public async Task DispatchAsync_Should_Continue_After_Handler_Failure_When_Policy_Is_Continue()
    {
        var calls = new List<string>();
        var handlers = new IDiscordGatewayDispatchHandler[]
        {
            new TestHandler("bad", order: 10, calls, shouldThrow: true, GatewayHandlerFailurePolicy.Continue),
            new TestHandler("good", order: 20, calls)
        };
        var orchestrator = CreateOrchestrator(handlers);
        var envelope = CreateEnvelope(new TestPayload("hello"));

        var result = await orchestrator.DispatchAsync(envelope, CancellationToken.None);

        Assert.True(result.HasFailures);
        Assert.Equal(new[] { "bad", "good" }, calls);
    }

    [Fact]
    public async Task DispatchAsync_Should_Stop_After_Handler_Failure_When_Policy_Is_StopPipeline()
    {
        var calls = new List<string>();
        var handlers = new IDiscordGatewayDispatchHandler[]
        {
            new TestHandler("bad", order: 10, calls, shouldThrow: true, GatewayHandlerFailurePolicy.StopPipeline),
            new TestHandler("never", order: 20, calls)
        };
        var orchestrator = CreateOrchestrator(handlers);
        var envelope = CreateEnvelope(new TestPayload("hello"));

        var result = await orchestrator.DispatchAsync(envelope, CancellationToken.None);

        Assert.True(result.HasFailures);
        Assert.Equal(new[] { "bad" }, calls);
    }

    private static DiscordGatewayDispatchOrchestrator CreateOrchestrator(IEnumerable<IDiscordGatewayDispatchHandler> handlers)
    {
        var catalog = new GatewayDispatchHandlerCatalog(
            handlers,
            NullLogger<GatewayDispatchHandlerCatalog>.Instance);

        return new DiscordGatewayDispatchOrchestrator(
            catalog,
            Options.Create(new GatewayDispatchOrchestrationOptions { HandlerTimeout = null }),
            new GatewayDispatchTelemetry(),
            NullLogger<DiscordGatewayDispatchOrchestrator>.Instance);
    }

    private static GatewayDispatchEnvelope CreateEnvelope(TestPayload payload) =>
        new("TEST_EVENT", payload, typeof(TestPayload), sequenceNumber: 42, DateTimeOffset.UtcNow);

    private sealed record TestPayload(string Value);

    private sealed class TestHandler : DiscordGatewayDispatchHandler<TestPayload>
    {
        private readonly List<string> _calls;
        private readonly bool _shouldThrow;

        public TestHandler(
            string name,
            int order,
            List<string> calls,
            bool shouldThrow = false,
            GatewayHandlerFailurePolicy failurePolicy = GatewayHandlerFailurePolicy.Continue)
        {
            HandlerName = name;
            Order = order;
            _calls = calls;
            _shouldThrow = shouldThrow;
            FailurePolicy = failurePolicy;
        }

        public override string HandlerName { get; }

        public override string EventName => "TEST_EVENT";

        public override int Order { get; }

        public override GatewayHandlerFailurePolicy FailurePolicy { get; }

        protected override ValueTask<GatewayHandlerExecutionResult> HandleAsync(
            TestPayload payload,
            GatewayDispatchEnvelope envelope,
            CancellationToken cancellationToken)
        {
            _calls.Add(HandlerName);
            if (_shouldThrow)
            {
                throw new InvalidOperationException("boom");
            }

            return ValueTask.FromResult(GatewayHandlerExecutionResult.Succeeded());
        }
    }
}
