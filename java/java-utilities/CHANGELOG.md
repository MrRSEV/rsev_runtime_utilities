# Changelog

## [2.0.0] - 2026

### Added

- Typed event registries, handler/listener contracts, asynchronous publishing, and a central event bus.
- Runtime subprocess lifecycle abstractions, stdout/stderr callbacks, health checks, and a process controller.
- Built-in process adapters for shell, Python, Node.js, .NET, and arbitrary executables.
- Dependency-free runtime API host, endpoint controller, request builder, API client, responses, and sessions.
- RuntimeContext accessors for the process controller and API host.

### Changed

- The Gradle project version is now `2.0.0`.
- The implementation continues to use only the Java standard library.

## [1.0.0] - Initial Release

- Dependency-free SMTP stack and runtime foundations.
