# Changelog

## [2.0.0] - 2026

### Added

- Typed event registries with handler/listener subscriptions and synchronous/asynchronous publishing.
- Runtime subprocess lifecycle abstractions, output callbacks, health checks, and a process controller.
- Built-in process adapters for shell, Python, Node.js, .NET, and arbitrary executables.
- Dependency-free runtime API host, endpoint controller, request builder, API client, responses, and sessions.
- RuntimeContext factories for the process controller and API host.

### Changed

- The Python package metadata is now version `2.0.0`.
- The existing `EventBus.subscribe/publish` API remains compatible with 1.x callers.
