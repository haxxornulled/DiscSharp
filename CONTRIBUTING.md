# Contributing

Thanks for helping improve DiscSharp.

## Before you change anything

1. Read [README.md](README.md) and [docs/README.md](docs/README.md) so the architecture and onboarding path stay aligned.
2. Check [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md) to avoid building something that is already done or explicitly deferred.
3. Keep changes scoped. Small, reviewable commits are easier to merge and easier to undo if the universe objects.

## Working rules

- Prefer existing patterns in the repo over introducing new ones.
- Keep public APIs documented with XML comments.
- Do not commit build outputs, test artifacts, or editor state.
- Update docs when behavior or composition changes.
- Favor explicit composition and typed models over hidden magic.

## Validation

Run the solution checks before opening a PR or pushing a non-trivial change:

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## Releasing packages

1. Bump the package version by tagging a release commit with a `v` prefix, for example `v0.1.0-preview.2`.
2. Let the GitHub Actions release workflow publish from that tag.
3. Make sure nuget.org has a Trusted Publishing policy for `.github/workflows/build.yml` and the `production` environment.
4. Do not add long-lived NuGet API keys back into the repo. We are trying to be clever in a useful way, not a cursed way.

## Commit style

Use short, imperative commit messages that describe the user-visible change, for example:

- `Document public methods`
- `Improve README and repo hygiene`
- `Add initial contributing guide`

## Questions and cleanup

If a change touches architecture, docs, or package surface area, prefer asking early rather than smuggling in a surprise. That keeps the project calm, which is the whole point.
