# Components and Modals

`DiscSharp.Rest.Components` models Discord message and modal components. The current model intentionally covers both legacy components and newer Components V2/modal component types so the library can evolve without serializer hacks.

## Namespaces

```csharp
using DiscSharp.Rest.Components;
using DiscSharp.Rest.Interactions;
```

## Buttons

```csharp
var joinButton = DiscordComponent.Button(
    customId: "raid:join;raidId=123",
    label: "Join Raid",
    style: DiscordButtonStyle.Success);

var row = DiscordComponent.ActionRow(joinButton);

var response = DiscordInteractionResponsePayload.Message(
    content: "Raid is forming.",
    ephemeral: false,
    components: [row]);
```

Use `LinkButton` for URL buttons and `PremiumButton` for SKU buttons. The normal `Button` factory rejects link and premium styles because those payload shapes are different.

## String selects

```csharp
var select = DiscordComponent.StringSelect(
    customId: "raid:role",
    options:
    [
        new DiscordSelectOption("Tank", "tank"),
        new DiscordSelectOption("Healer", "healer"),
        new DiscordSelectOption("Damage", "dps")
    ],
    placeholder: "Choose your role",
    minValues: 1,
    maxValues: 1);
```

## Entity selects

```csharp
var userSelect = DiscordComponent.EntitySelect(
    DiscordComponentType.UserSelect,
    customId: "raid:invite",
    placeholder: "Invite players");
```

Supported entity select types: `UserSelect`, `RoleSelect`, `MentionableSelect`, and `ChannelSelect`.

## Modals with Label components

Discord’s newer modal model uses `Label` as a wrapper around modal child components. Prefer this shape instead of relying on older text-input label fields.

```csharp
var raidName = DiscordComponent.LabelComponent(
    label: "Raid name",
    description: "Short name shown to players.",
    component: DiscordComponent.TextInput(
        customId: "raid:name",
        style: DiscordTextInputStyle.Short,
        minLength: 3,
        maxLength: 80,
        required: true));

var modal = DiscordInteractionResponsePayload.Modal(
    customId: "raid:create-modal",
    title: "Create Raid",
    components: [raidName]);
```

Validation rules currently enforced by factories:

| Field | Limit |
| --- | --- |
| Modal `custom_id` | 1-100 characters |
| Modal title | max 45 characters |
| Modal top-level components | 1-5 |
| Label text | max 45 characters |
| Label description | max 100 characters |
| Text input value | max 4000 characters |
| Text input placeholder | max 100 characters |

## Modal file upload

```csharp
var upload = DiscordComponent.LabelComponent(
    label: "Raid screenshot",
    description: "Attach logs or screenshot if needed.",
    component: DiscordComponent.FileUpload(
        customId: "raid:file",
        minValues: 0,
        maxValues: 3,
        required: false));
```

File Upload components belong in modals and should be wrapped in a Label.

## Components V2 note

Discord Components V2 uses the `IS_COMPONENTS_V2` flag. Once a message is sent with this flag, it changes how content, embeds, attachments, polls, and stickers behave for that message.

Do not casually set the flag on legacy message payloads. Use it deliberately when building a Components V2 message layout with text display/container/file/media components.

The current DiscSharp model includes types needed to track this system, but not every Components V2 factory has been implemented yet. Add new factories around `DiscordComponent` as endpoint support expands.

## Safe custom ID pattern

Use module/action IDs:

```text
raid:join;raidId=123
music:pause;guildId=456
```

Do not encode secrets, user tokens, OAuth tokens, or unbounded JSON in `custom_id`. Discord sends custom IDs back to your app; treat them as user-visible routing data.

## Serialization tests

Component and modal payloads are public contract. Any new component factory should come with tests that verify exact JSON shape.
