# RSEV.Utilities  
A universal, extensible, and future-proof .NET framework for plugins, controllers, modules, and system services.

RSEV.Utilities is a modular utility framework that provides developers with a stable foundation for building complex applications, services, or plugin systems.  
It combines clarity, extensibility, and architectural purity — with the goal of removing barriers and enabling productive development.

---

## Features

### Universal RuntimeContext
- Global configuration (`IConfig`)
- AssemblyLoader for dynamic plugin loading
- TypeDiscovery for automatic type detection
- PluginRegistry for activating/deactivating plugins
- MessageBusRegistry for registering and constructing custom message buses
- GlobalState for system-wide states
- Integrated logger (`ILogger`)

### Lifecycle System
- `IOnStart` / `IOnStartAsync`
- `IOnStop` / `IOnStopAsync`
- `IFirstRun` for one-time initialization
- Clean separation of start, stop, and setup logic

### Logging System
- `SystemLog` as an extensible standard logger
- Log rotation
- Colored console output
- Log levels: Debug, Info, Warning, Error, Critical
- Replaceable via `ILogger`

### Stable Shutdown & Error Handling
- `UnexpectedExitHandler` for controlled shutdown on errors
- `ShutdownSummary` for diagnostics and logging
- Recommendation to use custom `IOnStop` implementations

### Plugin & Module Architecture
- Dynamic loading of external assemblies
- Automatic discovery of controllers, plugins, and services
- Extensible without modifying the core system

### Event & Messaging Templates
- `IEventHandler<TEventArgs>` / `BaseEventHandler<TEventArgs>` for event processing
- `IEventListener<TEventArgs>` / `BaseEventListener<TEventArgs>` for event reception
- `IEvent<TEventArgs>` for event type definitions with metadata
- `IEventRegistry<TEventArgs>` / `EventRegistry<TEventArgs>` for event registration and publishing
- `IEventBus` / `BaseEventBus` for centralized multi-registry event bus implementations
- `IMessageBus` / `BaseMessageBus` for custom message-bus implementations
- `MessageBusRegistry` for registering, loading, and managing message buses

### Flexible Event Registration
- Register events independently without message bus binding
- Alternatively, create custom EventBus implementations and register via MessageBusRegistry
- Thread-safe handler and listener subscription/unsubscription
- Synchronous and asynchronous event publishing
- Framework users can extend with just 1-3 lines in custom RuntimeContext

### Process Management
- `RuntimeProcessController` for centralized subprocess lifecycle management
- `IRuntimeSubprocess` interface for subprocess contracts with exit codes and state tracking
- Process implementations: `DotNetProcess`, `NodeProcess`, `ShellProcess`, `ExecutableProcess`, `PythonProcess`
- Cross-platform executable resolution via `ExecutableResolver` utility
- Event-driven stdout/stderr handling with automatic log classification
- Generic health checks (`CheckHealthAsync`) with process-type-specific overrides
- Process-specific log level classification (`ProcessLogLevel`: Info, Warning, Error)
- Thread-safe process registry with discovery by name or state

### ApiBuilder (Lightweight API Host)
- `RuntimeApiHost` – a framework-native TCP-based API host, independent of ASP.NET Core
- `IRuntimeEndpoint` / `BaseRuntimeEndpoint` for defining routed endpoints with an `OnHandleAsync` hook
- `RuntimeEndpointController` for thread-safe endpoint registration and route resolution
- `EndpointRequest` / `EndpointResponse` models for request/response handling
- Manual HTTP/1.1-style parsing (request line, headers, Content-Length body) over raw TCP sockets
- Automatic exception handling: unhandled endpoint exceptions are converted to HTTP 500 responses
- TLS-ready via a reusable `Networking.Security` layer (`TlsOptions`, `TlsStreamFactory`) using only built-in `System.Net.Security` / `System.Security.Cryptography` classes
- TLS layer is protocol-agnostic and can be reused by other framework components beyond ApiBuilder

### RuntimeClient (Request Builder & API Client)
- `RuntimeApiClient` – high-level client counterpart to `RuntimeApiHost`, handling transport, requests, and sessions
- `RuntimeRequestBuilder` – fluent builder (`WithRoute`, `WithPayload`, `WithHeader`, `BindSession`, `Build`, `BuildBytes`) producing HTTP/1.1-compatible byte payloads consumable directly by `RuntimeApiHost`
- `RuntimeRequest` / `RuntimeResponse` models with automatic error detection (`IsError`) based on status code
- `RuntimeSession` for binding session state to requests, with an extensible `RenewAsync` hook for automatic session renewal
- `TcpTransport` / `TcpConnection` for raw TCP connectivity with optional TLS, reusing the existing `Networking.Security` layer
- `TlsSettings` – client-facing TLS configuration model that maps onto the shared `TlsOptions`
- Automatic 500-error recognition and JSON payload serialization via `System.Text.Json`

---

## Installation

```bash
dotnet add package RSEV.Utilities

```
