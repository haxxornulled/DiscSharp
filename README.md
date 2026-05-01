# DiscSharp

DiscSharp is an enterprise-grade .NET 10/C# Discord library under active development. The current rollup includes REST/API alignment primitives, interaction callback/followup support, Gateway dispatch orchestration, interaction pipeline modules, component/modal models, Raid Manager and Music Player interaction pack skeletons, Autofac modules, and tests.

## Start here

1. [docs/README.md](docs/README.md)
2. [docs/architecture/why-discsharp.md](docs/architecture/why-discsharp.md)
3. [docs/guides/getting-started.md](docs/guides/getting-started.md)
4. [docs/api/rest.md](docs/api/rest.md)
5. [docs/api/gateway-orchestration.md](docs/api/gateway-orchestration.md)
6. [docs/api/interactions.md](docs/api/interactions.md)
7. [docs/api/components-and-modals.md](docs/api/components-and-modals.md)
8. [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md)

## Product direction

DiscSharp is intentionally not a thin clone of existing .NET Discord libraries. The library is being shaped around Clean Architecture, Autofac-first composition, Serilog/OpenTelemetry-ready operations, typed REST/Gateway/interaction surfaces, multi-handler gateway orchestration, transport-neutral interaction modules, modern component/modal support, and enterprise-grade tests/docs.

The goal is not to brag. The goal is to make the architecture obvious, inspectable, testable, and production-safe.

## Build

```powershell
dotnet restore .\DiscSharp.slnx
dotnet build .\DiscSharp.slnx -c Release --no-restore
dotnet test .\DiscSharp.slnx -c Release --no-build
```

## Current status

See [docs/architecture/status-and-roadmap.md](docs/architecture/status-and-roadmap.md).
