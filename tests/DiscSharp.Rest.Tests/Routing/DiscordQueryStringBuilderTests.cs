using DiscSharp.Rest.Primitives;
using DiscSharp.Rest.Routing;
using Xunit;

namespace DiscSharp.Rest.Tests.Routing;

public sealed class DiscordQueryStringBuilderTests
{
    [Fact]
    public void Build_Should_Use_Repeated_Keys_For_Array_Parameters()
    {
        var query = new DiscordQueryStringBuilder()
            .AddSnowflakeArray("id", new[] { new DiscordSnowflake("123"), new DiscordSnowflake("456") })
            .AddBoolean("with_response", true)
            .Build();

        Assert.Equal("id=123&id=456&with_response=true", query);
    }
}
