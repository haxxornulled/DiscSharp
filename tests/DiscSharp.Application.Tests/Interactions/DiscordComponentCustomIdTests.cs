using DiscSharp.Application.Interactions;
using Xunit;

namespace DiscSharp.Application.Tests.Interactions;

public sealed class DiscordComponentCustomIdTests
{
    [Fact]
    public void ToString_Should_Serialize_Deterministically()
    {
        var customId = new DiscordComponentCustomId(
            "raid",
            "join",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["role"] = "tank",
                ["raidId"] = "123"
            });

        Assert.Equal("raid/join?raidId=123&role=tank", customId.ToString());
    }

    [Fact]
    public void Parse_Should_RoundTrip_CustomId()
    {
        var parsed = DiscordComponentCustomId.Parse("music/play?query=shine%20on");

        Assert.Equal("music", parsed.Module);
        Assert.Equal("play", parsed.Action);
        Assert.Equal("shine on", parsed.Arguments["query"]);
    }

    [Fact]
    public void TryParse_Should_Reject_Invalid_Segments()
    {
        var ok = DiscordComponentCustomId.TryParse("bad module/play", out _, out var error);

        Assert.False(ok);
        Assert.NotNull(error);
    }
}
