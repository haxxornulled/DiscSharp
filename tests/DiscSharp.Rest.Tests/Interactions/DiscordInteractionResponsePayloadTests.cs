using System.Text.Json;
using DiscSharp.Rest.Components;
using DiscSharp.Rest.Http;
using DiscSharp.Rest.Interactions;
using Xunit;

namespace DiscSharp.Rest.Tests.Interactions;

public sealed class DiscordInteractionResponsePayloadTests
{
    [Fact]
    public void Modal_Should_Serialize_Callback_Type_And_Components()
    {
        var payload = DiscordInteractionResponsePayload.Modal(
            "raid/create",
            "Create Raid",
            new[]
            {
                DiscordComponent.LabelComponent(
                    "Raid name",
                    DiscordComponent.TextInput("raid_name", DiscordTextInputStyle.Short, required: true))
            });

        var json = JsonSerializer.Serialize(payload, DiscordRestJson.Options);

        Assert.Contains("\"type\":9", json);
        Assert.Contains("\"custom_id\":\"raid/create\"", json);
        Assert.Contains("\"title\":\"Create Raid\"", json);
    }

    [Fact]
    public void DeferChannelMessage_Should_Use_Ephemeral_Flag_When_Requested()
    {
        var payload = DiscordInteractionResponsePayload.DeferChannelMessage(ephemeral: true);
        var json = JsonSerializer.Serialize(payload, DiscordRestJson.Options);

        Assert.Contains("\"type\":5", json);
        Assert.Contains("\"flags\":64", json);
    }
}
