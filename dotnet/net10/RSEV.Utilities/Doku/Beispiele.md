# Praxisbeispiele

Die Beispiele verwenden nur Typen aus dem aktuellen Quellstand.

## Konfiguration und Logging

```csharp
using RSEV.Utilities.Configuration;
using RSEV.Utilities.Logging;

var logger = new SystemLog("./log");
logger.Init();

var config = new JsonConfig();
config.Set("environment", "development");
config.Set("port", 8080);
await config.SaveAsync("appsettings.json");

logger.LogInfo($"Port: {config.Get<int>("port")}");
logger.Close();
```

`ConfConfig` speichert `key=value`, `JsonConfig` JSON und `YamlConfig` die
vom Parser unterstützte einfache YAML-Teilmenge.

## Eigener Runtime-Kontext

```csharp
using RSEV.Utilities.Configuration;
using RSEV.Utilities.Logging;
using RSEV.Utilities.Runtime;

public sealed class AppContext : BaseRuntimeContext
{
    protected override IConfig CreateDefaultConfig() => new JsonConfig();

    protected override ILogger CreateLogger() => new SystemLog("./runtime-log");
}

var context = new AppContext();
if (context.Logger is SystemLog systemLog)
    systemLog.Init();

context.Set("instance", Guid.NewGuid().ToString("N"));
context.IsRunning = true;
```

## HTTP-ähnlichen Endpoint definieren

```csharp
using System.Text;
using RSEV.Utilities.ApiBuilder;

public sealed class EchoEndpoint : BaseRuntimeEndpoint
{
    public EchoEndpoint() : base("/echo") { }

    protected override Task<EndpointResponse> OnHandleAsync(EndpointRequest request)
    {
        var text = Encoding.UTF8.GetString(request.Body);
        return Task.FromResult(
            EndpointResponse.FromText(200, $"received: {text}", "text/plain; charset=utf-8"));
    }
}

var host = new RuntimeApiHost();
host.Register(new EchoEndpoint());

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

await host.StartAsync(8080, cancellation.Token);
```

Der Endpoint bekommt keinen separaten HTTP-Methodenwert. Wenn die Anwendung
GET/POST unterscheiden muss, ist das mit dem aktuellen `EndpointRequest` nicht
direkt möglich und muss vorerst über eigene Header oder eine Codeänderung
gelöst werden.

## RuntimeClient mit JSON-Payload

```csharp
using RSEV.Utilities.RuntimeClient.Client;
using RSEV.Utilities.RuntimeClient.Requests;

var client = new RuntimeApiClient("127.0.0.1", 8080);
var request = new RuntimeRequestBuilder()
    .WithRoute("/echo")
    .WithPayload(new { message = "Hallo" })
    .WithHeader("X-Correlation-Id", Guid.NewGuid().ToString("N"))
    .Build();

var response = await client.SendAsync(request);
if (response.IsError)
    Console.Error.WriteLine(response.ErrorMessage);
else
    Console.WriteLine(response.GetBodyAsText());
```

`BuildBytes()` erzeugt die Rohbytes selbst. Der Client baut für jeden Aufruf
eine neue `TcpTransport`-Verbindung auf und schließt sie danach wieder.

## TLS-Server und TLS-Client

Serverseitig:

```csharp
using System.Security.Cryptography.X509Certificates;
using RSEV.Utilities.ApiBuilder;
using RSEV.Utilities.Networking.Security;

var certificate = new X509Certificate2("server.pfx", "passwort");
var tls = TlsOptions.ForServer(certificate);
var host = new RuntimeApiHost(tls);
host.Register(new HealthEndpoint());
await host.StartAsync(8443, cancellationToken);
```

Clientseitig:

```csharp
using RSEV.Utilities.RuntimeClient.Client;
using RSEV.Utilities.RuntimeClient.Networking;

var client = new RuntimeApiClient(
    "api.example.test",
    8443,
    useTls: true,
    tlsSettings: new TlsSettings
    {
        ValidateServerCertificate = true
    });
```

`ValidateServerCertificate = false` ist ausschließlich für lokale Tests
vertretbar. Das aktuelle `TlsSettings.ClientCertificate` wird beim Umwandeln
zu `TlsOptions` nicht weitergereicht; mTLS muss daher vor produktiver Nutzung
gegen den aktuellen Implementierungsstand getestet werden.

## Events

```csharp
using System;
using RSEV.Utilities.Events;

public sealed class OrderCreated : EventArgs
{
    public string OrderId { get; init; } = "";
}

public sealed class OrderHandler : BaseEventHandler<OrderCreated>
{
    protected override void OnHandle(object sender, OrderCreated args)
        => Console.WriteLine($"Order: {args.OrderId}");
}

var orders = new EventRegistry<OrderCreated>("order.created");
orders.Subscribe(new OrderHandler());
orders.Publish(new object(), new OrderCreated { OrderId = "A-100" });
```

Handler und Listener werden getrennt verwaltet und können jeweils synchron
oder asynchron implementiert werden.

## Command-Handler

```csharp
using RSEV.Utilities.Commands;
using RSEV.Utilities.Runtime;

public sealed class StatusCommand : BaseCommand
{
    public StatusCommand() : base("status", "Zeigt den Status") { }

    public override Task ExecuteAsync(IRuntimeContext context, params string[] arguments)
    {
        context.Logger.LogInfo("Status: OK");
        return Task.CompletedTask;
    }
}

var commands = new CommandHandler();
commands.Register(new StatusCommand());
await commands.ExecuteAsync("status", context);
```

Namen werden case-insensitiv behandelt. Eine Registrierung mit demselben Namen
überschreibt den bisherigen Command.

## Plugin laden

Ein klassisches Plugin benötigt einen parameterlosen Konstruktor, weil die
Discovery `Activator.CreateInstance` verwendet:

```csharp
using RSEV.Utilities.Plugins;
using RSEV.Utilities.Runtime;

public sealed class MetricsPlugin : IPlugin
{
    public string Name => "metrics";
    public string Description => "Metriken";
    public void OnEnable(IRuntimeContext context) => context.Logger.LogInfo("metrics on");
    public Task OnEnableAsync(IRuntimeContext context) { OnEnable(context); return Task.CompletedTask; }
    public void OnDisable(IRuntimeContext context) { }
    public Task OnDisableAsync(IRuntimeContext context) { OnDisable(context); return Task.CompletedTask; }
    public void OnReload(IRuntimeContext context) { }
    public Task OnReloadAsync(IRuntimeContext context) { OnReload(context); return Task.CompletedTask; }
}

context.PluginRegistry.LoadPlugins("./plugins", context);
```

Für Generator-Plugins werden `IGeneratorPlugin`, `GeneratorContext` und
`GeneratorPluginRegistry` verwendet. `GeneratorPluginLoader.Discover` lädt
DLLs im angegebenen Verzeichnis und registriert alle passenden Typen.

## Prozessverwaltung

```csharp
using RSEV.Utilities.Processes;

var processes = new RuntimeProcessController(context.Logger);
var worker = new PythonProcess(
    "worker",
    "worker.py",
    new[] { "--port", "9000" },
    context.Logger);

worker.OnStdOut += (_, line) => Console.WriteLine($"worker: {line}");
processes.Register(worker);

await processes.StartAsync(worker.ProcessId);
var health = await processes.CheckAllHealthAsync();
await processes.StopAsync(worker.ProcessId);
```

Die konkreten Adapter verwenden die Executables `dotnet`, `node`, `python`,
`pwsh` beziehungsweise `/bin/bash`. Diese Programme müssen im Zielsystem
vorhanden sein.

## Datenbankverbindung

```csharp
using RSEV.Utilities.Connection;
using RSEV.Utilities.Database;

var configuration = new ConnectionConfiguration
{
    ProviderInvariantName = "Microsoft.Data.Sqlite",
    ConnectionString = "Data Source=app.db",
    MaximumPoolSize = 10
};

var factory = new ConnectionFactory(new SqliteAdapter());
using var manager = new ConnectionManager(factory, configuration);
var connection = manager.Open(configuration);
try
{
    await connection.OpenAsync();
    // SQL-Befehle ausführen
}
finally
{
    manager.Return(connection);
}
```

`ConnectionManager.Open` holt eine Verbindung aus dem Pool oder erstellt sie
über den Adapter. Die Anwendung muss die konkrete Provider-Assembly bereitstellen.

## HMAC, SHA-256 und Session

```csharp
using System.Text;
using RSEV.Utilities.Crypto;
using RSEV.Utilities.Session;

var key = new KeyManager().Generate(32);
var hmac = new HamacProvider(key);
var data = Encoding.UTF8.GetBytes("payload");
var signature = hmac.Compute(data);
Console.WriteLine(hmac.Verify(data, signature));

var tokenProvider = new JwtTokenProvider(key);
var sessions = new SessionManager(tokenProvider);
var session = sessions.Create("user-123");
Console.WriteLine(sessions.IsValid(session));
```

Der Schlüssel muss sicher gespeichert werden. Der `SessionManager` validiert
lokal Signatur und Ablaufzeit der Tokens; er führt keinen serverseitigen
Refresh-Token-Speicher.

