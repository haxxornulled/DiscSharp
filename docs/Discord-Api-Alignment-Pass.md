# DiscSharp Discord API Alignment Pass

This pass continues from the uploaded green baseline (`DiscSharp.zip`) and adds a new `DiscSharp.Rest` project focused on API correctness rather than sample-only behavior.

## What this pass adds

- Discord API v10 route construction through `DiscordApiRoute` and `DiscordApiRoutes`.
- Discord-compatible query serialization through `DiscordQueryStringBuilder`, including repeated-key arrays such as `?id=123&id=456`.
- Central `DiscordApiOptions` with default `https://discord.com/api/v10/` targeting and a valid Discord-style `User-Agent`.
- Rate-limit header parsing through `DiscordRateLimitHeaders`:
  - `X-RateLimit-Limit`
  - `X-RateLimit-Remaining`
  - `X-RateLimit-Reset`
  - `X-RateLimit-Reset-After`
  - `X-RateLimit-Bucket`
  - `X-RateLimit-Global`
  - `X-RateLimit-Scope`
  - `Retry-After`
- Discord REST error modeling through `DiscordApiError` and `DiscordRestException`.
- Typed interaction callback and followup API through `IDiscordInteractionRestClient` and `DiscordInteractionRestClient`.
- Interaction callback payloads for:
  - `PONG`
  - immediate channel messages
  - deferred channel messages
  - deferred component updates
  - component message updates
  - autocomplete results
  - modal responses
- Components and modals model coverage for current Discord component values, including Components V2 and modal Label/File Upload support.
- Autofac-first REST composition through `DiscordRestModule`.
- Unit tests for route construction, query serialization, rate-limit parsing, interaction payloads, component serialization, and interaction HTTP requests.

## Alignment notes

Discord interaction responses must be sent over HTTP even when the interaction arrives through the Gateway. The `DiscordInteractionRestClient` makes this path explicit and keeps it separate from gateway dispatch orchestration.

Initial interaction responses are time-sensitive. The options object exposes the 3-second initial response deadline and 15-minute token lifetime so gateway/interaction modules can make defer-vs-work decisions without hard-coding magic numbers.

Rate limits must not be hard-coded. This pass parses the response headers and carries them into exceptions and telemetry so the existing retry/backoff layer can make bucket-aware decisions.

## Next recommended pass

1. Wire `DiscordInteractionRestClient` behind the existing `IInteractionResponseWriter` so gateway `INTERACTION_CREATE` handlers can write real callback responses.
2. Add typed application-command registration clients and DTOs.
3. Add webhook execute/edit/delete coverage shared by interaction followups.
4. Add route-bucket coordinator integration with the existing retry/backoff store.
5. Add source-generated JSON contexts once the payload shape stabilizes.
