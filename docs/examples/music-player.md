# Example: Music Player Interaction Flow

The Music Player pack is a pattern for interactive controls. It should eventually sit on top of a real voice/playback subsystem, but the interaction module should not own that transport.

## Component IDs

```text
music:play
music:pause
music:skip;guildId=123456789012345678
music:queue;page=2
```

Keep custom IDs stable and deterministic. Avoid encoding unbounded queue state in the ID.

## App service port

```csharp
public interface IMusicPlayerInteractionService
{
    ValueTask<MusicPlayerInteractionResult> HandleAsync(
        MusicInteractionRequest request,
        CancellationToken cancellationToken);
}
```

The app service owns queue state, voice connection lookup, authorization rules, track metadata, and playback state. The interaction module owns parsing the component ID, mapping to request intent, and returning a response plan.

## Registration

```csharp
builder.RegisterType<MusicPlayerInteractionService>()
    .As<IMusicPlayerInteractionService>()
    .SingleInstance();

builder.RegisterModule<MusicPlayerInteractionPackModule>();
```

## UI response pattern

Use immediate ephemeral responses for user-only state:

```csharp
return InteractionModuleResult.Respond(
    InteractionResponsePlan.Message("Skipped current track.", ephemeral: true));
```

Use update-message responses for shared player controls:

```csharp
return InteractionModuleResult.Respond(
    InteractionResponsePlan.UpdateMessage(
        content: "Now playing: ...",
        components: BuildPlayerControls()));
```

## Future voice boundary

Do not put FFmpeg, Opus, UDP, or voice Gateway code in the interaction module. The module talks to an application service. The service talks to a voice subsystem. The voice subsystem owns codecs and transport.
