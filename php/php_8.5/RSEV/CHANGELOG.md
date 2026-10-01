# Changelog

## [2.0.0] - 2026

### Added

- Typed event registries and event bus support with handler/listener subscriptions.
- Runtime subprocess lifecycle abstractions, output callbacks, health checks, and a process controller.
- Built-in process adapters for shell, Python, Node.js, .NET, and arbitrary executables.
- Dependency-free runtime API host, endpoint controller, request builder, API client, responses, and sessions.
- Composer package metadata with the canonical framework version.

### Changed

- Runtime architecture now exposes the same process and API concepts as the .NET 2.0 reference implementation.
- Existing `EventDispatcher` and `ProcessManager` APIs remain available for compatibility.
