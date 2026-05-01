using DiscSharp.Rest.Http;
using DiscSharp.Rest.Primitives;
using DiscSharp.Rest.Routing;
using Xunit;

namespace DiscSharp.Rest.Tests.Routing;

public sealed class DiscordApiRouteTests
{
    [Fact]
    public void CreateInteractionResponse_Should_Build_V10_Callback_Route()
    {
        var options = new DiscordApiOptions();
        var route = DiscordApiRoutes.CreateInteractionResponse(new DiscordSnowflake("111"), "token value");

        var uri = route.BuildUri(options, "with_response=true");

        Assert.Equal("https://discord.com/api/v10/interactions/111/token%20value/callback?with_response=true", uri.ToString());
    }
}
