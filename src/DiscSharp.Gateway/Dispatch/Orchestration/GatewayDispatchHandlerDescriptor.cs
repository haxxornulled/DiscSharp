namespace DiscSharp.Gateway.Dispatch.Orchestration;

/// <summary>
/// Describes a registered gateway dispatch handler.
/// </summary>
public sealed record GatewayDispatchHandlerDescriptor(
    string HandlerName,
    string EventName,
    Type PayloadType,
    int Order,
    GatewayHandlerFailurePolicy FailurePolicy);
