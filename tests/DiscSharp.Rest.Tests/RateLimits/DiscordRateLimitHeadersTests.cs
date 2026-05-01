using System.Net;
using DiscSharp.Rest.RateLimits;
using Xunit;

namespace DiscSharp.Rest.Tests.RateLimits;

public sealed class DiscordRateLimitHeadersTests
{
    [Fact]
    public void From_Should_Parse_Discord_RateLimit_Headers()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        response.Headers.TryAddWithoutValidation("X-RateLimit-Limit", "5");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Remaining", "0");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Reset", "1470173023.123");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Reset-After", "64.57");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Bucket", "abcd1234");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Global", "true");
        response.Headers.TryAddWithoutValidation("X-RateLimit-Scope", "global");
        response.Headers.TryAddWithoutValidation("Retry-After", "65");

        var headers = DiscordRateLimitHeaders.From(response);

        Assert.Equal(5, headers.Limit);
        Assert.Equal(0, headers.Remaining);
        Assert.Equal("abcd1234", headers.Bucket);
        Assert.True(headers.IsGlobal);
        Assert.Equal(DiscordRateLimitScope.Global, headers.Scope);
        Assert.Equal(TimeSpan.FromSeconds(65), headers.RetryAfter);
        Assert.Equal(TimeSpan.FromSeconds(64.57), headers.ResetAfter);
    }
}
