# RSEV.Utilities – Entwicklerdokumentation

Diese Dokumentation beschreibt den aktuellen Quellstand des Pakets
`RSEV.Utilities` und soll die tägliche Verwendung des Frameworks ohne
Quellcode-Recherche ermöglichen.

> **Gültigkeit:** Die API-Referenz wurde aus dem aktuellen Projektstand
> erstellt. Das Projekt heißt zwar im Verzeichnis `net10`, kompiliert laut
> `RSEV.Utilities.csproj` derzeit aber für **.NET 8 (`net8.0`)**.

## Dokumentationsstruktur

| Datei | Inhalt |
|---|---|
| [API-Referenz](API-Referenz.md) | Alle öffentlichen Namespaces, Klassen, Interfaces, Enums, Properties und Methoden |
| [Beispiele](Beispiele.md) | Kopierbare Einstiegsszenarien für die wichtigsten Module |
| [Architektur](Architektur.md) | Zusammenspiel der Module und empfohlene Integrationsreihenfolge |
| [Betrieb und Sicherheit](Betrieb-und-Sicherheit.md) | Fehlerbilder, Sicherheitsregeln und bekannte Eigenschaften des aktuellen Codes |
| [README.txt](README.txt) | Ältere DokuWiki-Kurzfassung; die Markdown-Dokumentation ist die ausführliche Referenz |
| [API-Referenz.txt](API-Referenz.txt) | Ältere Kurzreferenz; für Signaturen bitte `API-Referenz.md` verwenden |

## Was ist RSEV.Utilities?

`RSEV.Utilities` ist ein modulares .NET-Framework für Anwendungen, Dienste,
Plugin-Systeme und Runtime-Komponenten. Es bündelt bewusst kleine, lose
gekoppelte Bausteine:

- zentraler Runtime-Kontext mit Konfiguration, Logging, Discovery und Lifecycle,
- dynamisches Laden von Assemblies, Plugins und Message-Bussen,
- typisierte Events, Commands und Controller-Lifecycle,
- ein leichtgewichtiger TCP-/HTTP-API-Host samt Client,
- TCP/TLS-Abstraktionen,
- Prozessverwaltung für .NET, Node.js, Python, Shell und beliebige Executables,
- Datenbank- und Connection-Abstraktionen,
- kryptografische Provider und einfache Session-/Token-Verwaltung,
- SMTP-/lokaler Mailversand.

Das Paket verwendet im Projekt selbst ausschließlich .NET-Bibliotheken. Für
eine konkrete Datenbank muss die Anwendung zusätzlich den passenden ADO.NET-
Provider installieren und bei `DbProviderFactories` registrieren.

## Installation

```bash
dotnet add package RSEV.Utilities
```

Für die lokale Quellcodeversion:

```bash
dotnet build RSEV.Utilities.csproj
```

Die Paketmetadaten des aktuellen Projekts sind:

| Eigenschaft | Wert |
|---|---|
| Package ID | `RSEV.Utilities` |
| Version | `3.0.0` |
| Target Framework | `net8.0` |
| Nullable | aktiviert |
| Implicit usings | aktiviert |
| Lizenz | MIT |

## Schnellstart

### 1. Runtime-Kontext ableiten

`BaseRuntimeContext` ist abstrakt. Für eine Anwendung wird normalerweise eine
kleine konkrete Klasse angelegt:

```csharp
using RSEV.Utilities.Runtime;

public sealed class ApplicationContext : BaseRuntimeContext
{
    // Für Standardverhalten ist keine weitere Implementierung erforderlich.
}
```

Danach können die zentralen Dienste verwendet werden:

```csharp
var context = new ApplicationContext();

context.Set("environment", "production");
context.Logger.LogInfo("Anwendung wird gestartet");
context.IsRunning = true;

var environment = context.Get<string>("environment");
```

Wenn `SystemLog` tatsächlich in eine Datei schreiben soll, muss er vor der
ersten Dateiausgabe mit `Init()` initialisiert werden. Der Standard-Logger des
Kontexts wird durch `CreateLogger()` erzeugt und kann in einer abgeleiteten
Context-Klasse überschrieben werden.

### 2. Konfiguration laden

```csharp
using RSEV.Utilities.Configuration;

IConfig config = new JsonConfig();
config.Set("port", 8080);
await config.SaveAsync("appsettings.json");
await config.LoadAsync("appsettings.json");

int port = config.Get<int>("port");
```

Verfügbare Speicherformate sind `ConfConfig`, `JsonConfig` und `YamlConfig`.
Die Konverter sind bewusst leichtgewichtig; Details und Grenzen stehen in der
[API-Referenz](API-Referenz.md) und unter [Betrieb und Sicherheit](Betrieb-und-Sicherheit.md).

### 3. Endpoint registrieren

```csharp
using RSEV.Utilities.ApiBuilder;

public sealed class HealthEndpoint : BaseRuntimeEndpoint
{
    public HealthEndpoint() : base("/health") { }

    protected override Task<EndpointResponse> OnHandleAsync(EndpointRequest request)
        => Task.FromResult(EndpointResponse.FromText(200, "ok"));
}

var host = new RuntimeApiHost();
host.Register(new HealthEndpoint());

using var stop = new CancellationTokenSource();
await host.StartAsync(8080, stop.Token);
```

Der Host spricht ein kleines HTTP/1.1-Subset über TCP. Er ist kein Ersatz für
ASP.NET Core: Es werden nur Request-Line, Header und ein optionaler Body über
`Content-Length` verarbeitet. Die HTTP-Methode wird aktuell nicht an
`EndpointRequest` weitergegeben.

### 4. Client verwenden

```csharp
using RSEV.Utilities.RuntimeClient.Client;
using RSEV.Utilities.RuntimeClient.Requests;

var client = new RuntimeApiClient("127.0.0.1", 8080);
var response = await client.SendAsync(
    new RuntimeRequestBuilder()
        .WithRoute("/health")
        .WithPayload(new { ping = true })
        .Build());

Console.WriteLine(response.StatusCode);
Console.WriteLine(response.GetBodyAsText());
```

Der Builder erzeugt standardmäßig einen `POST`-Request mit JSON-Payload und
`Content-Length`. Für TLS werden `TlsSettings` und ein TLS-fähiger
`RuntimeApiHost` benötigt.

## Namespace-Übersicht

| Namespace | Zweck | Wichtige Typen |
|---|---|---|
| `RSEV.Utilities.Api` | unabhängiger `HttpClient`-Vertrag | `IApiClient`, `HttpApiClient`, `RetryPolicy`, `RateLimitPolicy` |
| `RSEV.Utilities.ApiBuilder` | eingebauter TCP-/HTTP-Host | `RuntimeApiHost`, `IRuntimeEndpoint`, `EndpointRequest`, `EndpointResponse` |
| `RSEV.Utilities.Commands` | Commands registrieren und ausführen | `ICommand`, `BaseCommand`, `CommandHandler` |
| `RSEV.Utilities.Communication` | Mailoptionen | `MailOptions` |
| `RSEV.Utilities.Communication.RSEV.Utilities.Communication` | aktuell tatsächlich kompilierter Namespace von `SystemMail` | `SystemMail` |
| `RSEV.Utilities.Configuration` | Konfigurationsspeicher | `IConfig`, `ConfConfig`, `JsonConfig`, `YamlConfig` |
| `RSEV.Utilities.Connection` | Erzeugung und Pooling von `DbConnection` | `ConnectionFactory`, `ConnectionPool`, `ConnectionManager` |
| `RSEV.Utilities.Controllers` | synchroner/asynchroner Controller-Lifecycle | `IController`, `BaseController` |
| `RSEV.Utilities.Crypto` | Hashing, HMAC und Schlüssel | `ICryptoProvider`, `HamacProvider`, `Sha256Provider`, `KeyManager` |
| `RSEV.Utilities.Database` | providerneutrale DB-Adapter | `IDatabaseAdapter`, `SqliteAdapter`, `SqlServerAdapter`, `MySqlAdapter`, `PostgreSqlAdapter` |
| `RSEV.Utilities.Events` | typisierte Events und Event-Busse | `EventRegistry<T>`, `IEventHandler<T>`, `BaseEventBus` |
| `RSEV.Utilities.Lifecycle` | Start-, Stop- und First-Run-Verträge | `IOnStart`, `IOnStop`, `IFirstRun` |
| `RSEV.Utilities.Loading` | Assembly- und Typ-Discovery | `AssemblyLoader`, `TypeDiscovery` |
| `RSEV.Utilities.Logging` | Loggervertrag und Standardlogger | `ILogger`, `SystemLog` |
| `RSEV.Utilities.Messaging` | Lifecycle-fähige Message-Busse | `IMessageBus`, `BaseMessageBus`, `MessageBusRegistry` |
| `RSEV.Utilities.Networking.Security` | wiederverwendbare TLS-Schicht | `TlsOptions`, `TlsStreamFactory` |
| `RSEV.Utilities.Plugins` | Plugins und Generator-Erweiterungen | `PluginRegistry`, `GeneratorPluginRegistry`, `IGeneratorPlugin` |
| `RSEV.Utilities.Processes` | verwaltete externe Prozesse | `IRuntimeSubprocess`, `RuntimeProcessController`, Prozessadapter |
| `RSEV.Utilities.Runtime` | zentraler Anwendungskontext und Shutdown | `IRuntimeContext`, `BaseRuntimeContext`, `UnexpectedExitHandler` |
| `RSEV.Utilities.RuntimeClient.Client` | Client-Fassade für den Runtime-Host | `RuntimeApiClient` |
| `RSEV.Utilities.RuntimeClient.Networking` | TCP-/TLS-Clienttransport | `TcpTransport`, `TcpConnection`, `TlsSettings` |
| `RSEV.Utilities.RuntimeClient.Requests` | Requests und Responses | `RuntimeRequest`, `RuntimeRequestBuilder`, `RuntimeResponse` |
| `RSEV.Utilities.RuntimeClient.Session` | clientseitige Sessionbindung | `RuntimeSession` |
| `RSEV.Utilities.Session` | Access-/Refresh-Token und JWT | `JwtTokenProvider`, `SessionManager`, `SessionContext` |

## Grundregeln für die Verwendung

1. **Lebensdauer beachten:** Streams, TCP-Verbindungen, Datenbankverbindungen
   und Logger deterministisch schließen.
2. **CancellationToken weiterreichen:** Besonders bei API-Hosts, TCP/TLS,
   Prozessstarts und langen I/O-Aufrufen.
3. **Plugins nur aus vertrauenswürdigen Verzeichnissen laden:**
   `Assembly.LoadFrom` führt fremden Code im Prozess aus.
4. **TLS-Zertifikatsprüfung nicht abschalten:**
   `AllowInvalidCertificates` beziehungsweise `ValidateServerCertificate` nur
   für lokale Tests verwenden.
5. **Schlüssel und Tokens niemals loggen:** Auch nicht in Debug-Ausgaben.
6. **Rückgabewerte prüfen:** Viele Registry- und Lifecycle-Methoden melden
   Fehlschläge als `false`, `null` oder leere Listen statt als Exception.

## Vollständigkeit der API-Referenz

Die [API-Referenz](API-Referenz.md) enthält die aus dem aktuellen Build
ermittelten öffentlichen Typen. Dazu gehören auch verschachtelte Enums wie
`SystemLog.LogLevel` und die beiden Lifecycle-Typen im Namespace
`RSEV.Utilities.Lifecycle`.

