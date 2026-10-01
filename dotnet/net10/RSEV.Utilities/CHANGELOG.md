# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.0.0] - 2026

### Added

#### Process Management System
- **`RuntimeProcessController`** – Centralized manager for subprocess lifecycle
  - Register/unregister processes
  - Start/stop individual processes or all processes
  - Health check aggregation
  - Discovery by name or state
  - Thread-safe process registry

- **`IRuntimeSubprocess`** – Interface for managed subprocesses
  - Properties: ProcessId, Name, Executable, Arguments, State, ExitCode, Properties
  - Events: OnStdOut, OnStdErr
  - Methods: StartAsync, StopAsync, RestartAsync, CheckHealthAsync (default: `return IsRunning`)

- **`RuntimeSubprocessBase`** – Abstract base class for subprocess implementations
  - Lifecycle state machine (Created → Starting → Running → Stopping → Stopped or Faulted)
  - Automatic stdout/stderr pumping with event raising
  - Log classification hook method (`ClassifyLogLine`)
  - Protection for illegal state transitions
  - Logging support with `LogInfo`, `LogWarning`, `LogError`

- **Process Implementations:**
  - **`DotNetProcess`** – Manages .NET DLL execution via `dotnet "<dll>"`
  - **`NodeProcess`** – Manages Node.js script execution via `node <script>`
  - **`ShellProcess`** – Cross-platform shell command execution (bash/pwsh)
  - **`ExecutableProcess`** – Generic executable runner with resolver integration
  - **`PythonProcess`** – Manages Python script execution via `python <script>`

- **`ExecutableResolver`** – Utility for cross-platform executable resolution
  - Resolves executables via environment variable or fallback paths
  - Platform-specific path resolution (Windows/Linux)
  - Unix permission handling for Linux executables

- **Process Enums:**
  - `SubprocessState` – States: Created, Starting, Running, Stopping, Stopped, Faulted
  - `ProcessLogLevel` – Log classification: Info, Warning, Error

- **Runtime Context Integration:**
  - `IRuntimeContext` extended with `ProcessController` property
  - `BaseRuntimeContext` includes `CreateProcessController()` factory method

#### ApiBuilder (Lightweight API Host)
- **`RuntimeApiHost`** – Framework-native TCP-based API host, independent of ASP.NET Core
  - Manual HTTP/1.1-style request parsing (request line, headers, Content-Length body)
  - Automatic exception handling: unhandled endpoint exceptions are converted to HTTP 500 responses
  - Internally owns a `RuntimeEndpointController` instance
  - Optional TLS support via the `Networking.Security` layer

- **`IRuntimeEndpoint`** – Interface for routed endpoints
  - Properties: Route
  - Methods: HandleAsync(EndpointRequest) => Task\<EndpointResponse\>

- **`BaseRuntimeEndpoint`** – Abstract base class with `OnHandleAsync` hook method, analogous to `BaseEventHandler`

- **`RuntimeEndpointController`** – Thread-safe registry for endpoints
  - Register/Unregister/Resolve by route
  - Enumerate all registered endpoints

- **`EndpointRequest` / `EndpointResponse`** – Plain data models for request/response handling
  - `EndpointResponse.FromText(...)` convenience factory for simple text responses

- **`Networking.Security` layer** – Reusable, protocol-agnostic TLS support
  - **`TlsOptions`** – Configuration for certificates, protocols, and client/server mode
  - **`TlsStreamFactory`** – Wraps `NetworkStream` into an authenticated `SslStream` (server/client) using only `System.Net.Security` and `System.Security.Cryptography` classes
  - Designed to be reused by other framework components beyond ApiBuilder

- **Runtime Context Integration:**
  - `IRuntimeContext` extended with `ApiHost` property
  - `BaseRuntimeContext` includes `CreateApiHost()` factory method

#### RuntimeClient (Request Builder & API Client)
- **`RuntimeApiClient`** – High-level client counterpart to `RuntimeApiHost`
  - `CallAsync(route, payload)` – builds and sends a request in one call
  - `SendAsync(RuntimeRequest)` – sends a pre-constructed request
  - `SetSession(RuntimeSession)` – binds a session to all subsequent requests
  - Automatically detects HTTP 500 (and other 4xx/5xx) responses via `RuntimeResponse.IsError`
  - Automatically renews an expired session (`RuntimeSession.RenewAsync`) before sending

- **`RuntimeRequestBuilder`** – Fluent builder for Engine-conformant requests
  - `WithRoute`, `WithPayload`, `WithHeader`, `BindSession`, `Build`, `BuildBytes`
  - Serializes payloads as JSON via `System.Text.Json`
  - `BuildBytes()` produces an HTTP/1.1-compatible byte payload directly consumable by `RuntimeApiHost`

- **`RuntimeRequest` / `RuntimeResponse`** – Data models for client requests/responses
  - `RuntimeRequest`: Route, Payload, Headers, SessionId
  - `RuntimeResponse`: StatusCode, Body, Headers, IsError (derived), ErrorMessage, `GetBodyAsText()`

- **`RuntimeSession`** – Session binding for requests
  - Properties: SessionId, ExpiresAt, Metadata, IsExpired
  - `AttachToRequest(RuntimeRequest)` – attaches the session id to a request
  - `RenewAsync()` – virtual extension point for custom session renewal logic

- **`TcpTransport` / `TcpConnection`** – Raw TCP transport with optional TLS
  - `ConnectAsync(host, port, useTls, tlsSettings)` – establishes a connection, optionally TLS-secured
  - `SendAsync(byte[])` / `ReceiveAsync()` – raw byte transmission
  - Reuses the existing `Networking.Security.TlsStreamFactory` for the TLS handshake

- **`TlsSettings`** – Client-facing TLS configuration model
  - Properties: ClientCertificate, ValidateServerCertificate, AllowedProtocols
  - `ToTlsOptions(targetHost)` – maps onto the shared `TlsOptions` used by the TLS layer

### Changed

- Event system architecture fully refactored for flexible registration patterns
- README.md updated with complete documentation of event system, process management, ApiBuilder, and RuntimeClient features
- Framework architecture now supports flexible event registration patterns
- `IRuntimeContext` and `BaseRuntimeContext` extended with `ProcessController` and `ApiHost` properties
- **`IEvent<TEventArgs>`** – Interface for defining event types with metadata (name, description, event args type)
- **`IEventRegistry<TEventArgs>`** – Interface for event registration management with handler/listener subscription
- **`EventRegistry<TEventArgs>`** – Concrete implementation of event registry with:
  - Thread-safe handler and listener management
  - Synchronous and asynchronous event publishing
  - Subscribe/Unsubscribe methods for handlers and listeners
  - Read-only collection access to registered components

#### Event Bus Integration
- **`IEventBus`** – Interface extending `IMessageBus` for centralized event management
  - Register/retrieve/check event registries by event name and type
  - Get all registered event names
  - Integrates seamlessly with `MessageBusRegistry`
- **`BaseEventBus`** – Abstract base class for custom event bus implementations
  - Manages multiple `EventRegistry` instances
  - Thread-safe registry dictionary with type-safe keys
  - Extensible hook methods for initialization and shutdown

#### Handler and Listener Templates
- **`IEventHandler<TEventArgs>`** – Interface for event handlers with sync/async processing
- **`BaseEventHandler<TEventArgs>`** – Base class with hook methods (`OnHandle`, `OnHandleAsync`)
- **`IEventListener<TEventArgs>`** – Interface for event listeners with sync/async reception
- **`BaseEventListener<TEventArgs>`** – Base class with hook methods (`OnListen`, `OnListenAsync`)

#### Message Bus Extensions
- **`MessageBusRegistry`** – Extended to support loading and managing message buses from directories
- **`IRuntimeContext`** – Added `MessageBusRegistry` property
- **`BaseRuntimeContext`** – Added `CreateMessageBusRegistry()` factory method

### How to Use (Framework Users)

#### Option 1: Simple Event Registration (without Message Bus)
```csharp
// Define your event args
public class MyEventArgs : EventArgs { }

// Create and use registry independently
var registry = new EventRegistry<MyEventArgs>("MyEvent");
var handler = new MyHandler();
var listener = new MyListener();

registry.Subscribe(handler);
registry.Subscribe(listener);

// Publish event
registry.Publish(this, new MyEventArgs());
```

#### Option 2: Custom EventBus with Message Bus Integration
```csharp
// Create custom event bus extending BaseEventBus
public class MyEventBus : BaseEventBus
{
	public MyEventBus() : base("MyEventBus", "Custom event bus for my application") { }

	protected override void OnInitialize(IRuntimeContext context)
	{
		// Initialize registries
		base.OnInitialize(context);
		RegisterEventRegistry(new EventRegistry<MyEventArgs>("MyEvent"));
	}
}

// In your custom RuntimeContext
public class MyRuntimeContext : BaseRuntimeContext
{
	protected override void OnInitialize()
	{
		base.OnInitialize();

		// Register your event bus
		var eventBus = new MyEventBus();
		MessageBusRegistry.Register(eventBus);

		// Access registries
		var registry = eventBus.GetRegistry<MyEventArgs>("MyEvent");
		registry.Subscribe(handler);
	}
}
```

#### Option 3: Process Management

```csharp
// Create and register a process
var process = new DotNetProcess("MyApp", "path/to/app.dll");
ProcessController.Register(process);

// Start/stop
await ProcessController.StartAsync("MyApp");
await ProcessController.StopAsync("MyApp");

// Subscribe to output events
process.OnStdOut += (sender, line) => Console.WriteLine($"OUT: {line}");
process.OnStdErr += (sender, line) => Console.WriteLine($"ERR: {line}");
```

#### Option 4: ApiBuilder Endpoint

```csharp
// Define a custom endpoint
public class StatusEndpoint : BaseRuntimeEndpoint
{
	public StatusEndpoint() : base("/api/status") { }

	protected override Task<EndpointResponse> OnHandleAsync(EndpointRequest request)
	{
		return Task.FromResult(EndpointResponse.FromText(200, "OK"));
	}
}

// Register and start the host
ApiHost.Register(new StatusEndpoint());
await ApiHost.StartAsync(port: 8080);
```

---

## [2.0.0] - 2026

### Added
- **Message Bus Architecture** with `IMessageBus` and `BaseMessageBus`
- **MessageBusRegistry** for registering and managing message buses
- Integration with `RuntimeContext` for centralized bus management
- Event Handler and Listener base templates

### Fixed
- Project structure and namespace consistency

---

## [2.0.0] - 2026

### Added
- Initial framework release
- Universal `RuntimeContext` with global services
- Plugin architecture with `PluginRegistry`
- Lifecycle system (`IOnStart`, `IOnStop`, `IFirstRun`)
- Logging system with `SystemLog`
- Configuration system with multiple formats (JSON, YAML, CONF)
- Command system with `CommandHandler`
- Controller architecture with `BaseController`
- Assembly loading and type discovery utilities
- Error handling with `UnexpectedExitHandler` and `ShutdownSummary`
