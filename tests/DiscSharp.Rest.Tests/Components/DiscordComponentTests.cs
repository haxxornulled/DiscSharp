using System.Text.Json;
using DiscSharp.Rest.Components;
using DiscSharp.Rest.Http;
using Xunit;

namespace DiscSharp.Rest.Tests.Components;

public sealed class DiscordComponentTests
{
    [Fact]
    public void LabelComponent_Should_Serialize_New_Modal_Label_Shape()
    {
        var component = DiscordComponent.LabelComponent(
            "Feedback",
            DiscordComponent.TextInput("feedback", DiscordTextInputStyle.Paragraph, minLength: 1, maxLength: 4000, required: true),
            "Tell us what happened.");

        var json = JsonSerializer.Serialize(component, DiscordRestJson.Options);

        Assert.Contains("\"type\":18", json);
        Assert.Contains("\"component\":", json);
        Assert.Contains("\"type\":4", json);
        Assert.Contains("\"custom_id\":\"feedback\"", json);
    }

    [Fact]
    public void Button_Should_Reject_Link_Style_When_CustomId_Is_Required()
    {
        Assert.Throws<ArgumentException>(() => DiscordComponent.Button("x", "Open", DiscordButtonStyle.Link));
    }
}
