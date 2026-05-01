using System.Net;
using DiscSharp.Rest.Http;
using DiscSharp.Rest.Interactions;
using DiscSharp.Rest.Primitives;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DiscSharp.Rest.Tests.Interactions;

public sealed class DiscordInteractionRestClientTests
{
    [Fact]
    public async Task CreateInteractionResponseAsync_Should_Send_V10_Callback_With_Response_Query()
    {
        using var handler = new CapturingHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"interaction\":{\"id\":\"111\"}}")
        });
        using var httpClient = new HttpClient(handler);
        var client = CreateClient(httpClient);

        var response = await client.CreateInteractionResponseAsync(
            new DiscordSnowflake("111"),
            "abc",
            DiscordInteractionResponsePayload.Message("hello", ephemeral: true),
            withResponse: true,
            CancellationToken.None);

        Assert.NotNull(response);
        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("https://discord.com/api/v10/interactions/111/abc/callback?with_response=true", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("DiscordBot", handler.LastRequest.Headers.UserAgent.ToString());
        Assert.Contains("\"type\":4", handler.LastContent);
        Assert.Contains("\"flags\":64", handler.LastContent);
    }

    private static DiscordInteractionRestClient CreateClient(HttpClient httpClient) =>
        new(
            httpClient,
            Options.Create(new DiscordApiOptions()),
            new DiscordRestTelemetry(),
            NullLogger<DiscordInteractionRestClient>.Instance);

    private sealed class CapturingHandler : HttpMessageHandler, IDisposable
    {
        private readonly HttpResponseMessage _response;

        public CapturingHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        public HttpRequestMessage? LastRequest { get; private set; }

        public string LastContent { get; private set; } = string.Empty;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null)
            {
                LastContent = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            }

            return _response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _response.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
