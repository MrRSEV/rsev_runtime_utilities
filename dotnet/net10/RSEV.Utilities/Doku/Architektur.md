# Architektur und Integrationsmodell

## Überblick

RSEV.Utilities besteht aus drei Ebenen:

```text
Anwendung / eigener RuntimeContext
        |
        +-- Runtime, Lifecycle, Logging, Configuration
        |
        +-- Loading, Plugins, Messaging, Events, Commands, Controllers
        |
        +-- ApiBuilder / RuntimeClient / Networking.Security
        |
        +-- Processes / Connection / Database / Crypto / Session / Communication
```

Die Module sind überwiegend eigenständig. `IRuntimeContext` bündelt sie als
gemeinsame Laufzeitoberfläche, erzwingt aber keine Dependency-Injection-
Bibliothek und startet keine Komponenten automatisch.

## Empfohlene Startreihenfolge

1. Konkreten `BaseRuntimeContext` ableiten.
2. Logger erzeugen und bei `SystemLog` `Init()` aufrufen.
3. Konfiguration laden und globale Werte mit `Set()` ablegen.
4. Vertrauenswürdige Plugin-/Message-Bus-Verzeichnisse laden.
5. Controller, Endpoints und Prozesse registrieren.
6. `IsRunning = true` setzen und bei Bedarf `IOnStart`/`IOnStartAsync`
   ausführen.
7. Beim Stop erst neue Arbeit verhindern, dann Prozesse, Message-Busse,
   Plugins, Verbindungen und Logger schließen.

Das Framework besitzt keine zentrale `StartAll()`-Methode für sämtliche
Subsysteme. Die Anwendung bleibt für Reihenfolge und Fehlerstrategie
verantwortlich.

## Runtime-Kontext

`IRuntimeContext` stellt folgende Dienste bereit:

- `IConfig Config`
- `ILogger Logger`
- `AssemblyLoader AssemblyLoader`
- `TypeDiscovery TypeDiscovery`
- `PluginRegistry PluginRegistry`
- `MessageBusRegistry MessageBusRegistry`
- `RuntimeProcessController ProcessController`
- `RuntimeApiHost ApiHost`
- `IDictionary<string, object> GlobalState`
- `bool IsRunning`

`BaseRuntimeContext` erzeugt Standardinstanzen über überschreibbare Factory-
Methoden (`CreateDefaultConfig`, `CreateLogger`, `CreateApiHost` usw.). Dadurch
kann eine Anwendung eigene Implementierungen einschleusen, ohne den gesamten
Kontext neu zu schreiben.

## Lifecycle-Verträge

Es gibt mehrere, bewusst getrennte Lifecycle-Modelle:

| Vertrag | Verwendung |
|---|---|
| `IOnStart` | Startlogik synchron und asynchron |
| `IOnStop` | Aufräumen synchron und asynchron |
| `IFirstRun` | Einmaliges Setup zusätzlich zu `IOnStart` |
| `IController` | Initialisieren und Herunterfahren eines Controllers |
| `IPlugin` | Aktivieren, Deaktivieren und Reload eines Plugins |
| `IMessageBus` | Initialisieren und Herunterfahren eines Message-Busses |
| `IRuntimeSubprocess` | Start, Stop, Restart und Health eines externen Prozesses |

Diese Verträge sind nicht automatisch miteinander verdrahtet. Eine Anwendung
entscheidet, welcher Registry- oder Lifecycle-Aufruf zu welchem Zeitpunkt
erfolgt.

## Event- und Messaging-Fluss

```text
EventRegistry<TEventArgs>
    +-- IEventHandler<TEventArgs>.Handle / HandleAsync
    +-- IEventListener<TEventArgs>.Listen / ListenAsync

BaseEventBus : BaseMessageBus
    +-- mehrere EventRegistry<TEventArgs>, Schlüssel aus Name + Argumenttyp

MessageBusRegistry
    +-- lädt IMessageBus aus DLLs
    +-- initialisiert und beendet registrierte Busse
```

`EventRegistry<T>` kopiert die Handler-/Listenerlisten vor dem Publizieren.
Dadurch kann während eines Callbacks registriert oder deregistriert werden,
ohne die aktuelle Iteration zu verändern. Ausnahmen einzelner Handler und
Listener werden im aktuellen Implementierungsstand bewusst verschluckt.

## API-Host und Client

Der Host verwendet TCP und eine minimale HTTP-artige Darstellung:

```text
RuntimeRequestBuilder
      | BuildBytes()
      v
TCP/TLS -> RuntimeApiHost -> RuntimeEndpointController -> IRuntimeEndpoint
      ^                                             |
      +------------- RuntimeResponse ---------------+
```

Der Host liest:

- eine Request-Line wie `POST /health HTTP/1.1`,
- Header bis `\r\n\r\n`,
- optional `Content-Length` Bytes.

Er verarbeitet keine vollständige HTTP-Spezifikation. Es gibt kein Routing-
Template, keine Middleware und keine automatische JSON-Deserialisierung.
Der Endpoint erhält `Body` als Bytes und entscheidet selbst, wie der Inhalt
interpretiert wird.

## Prozesse

Alle Prozessadapter erben von `RuntimeSubprocessBase`:

```text
Created -> Starting -> Running -> Stopping -> Stopped
                    \-> Faulted
```

`RuntimeProcessController` registriert Prozesse nach ihrer automatisch
erzeugten `ProcessId`. Die Suche nach Name kann mehrere Treffer liefern;
`StartByNameAsync` und `StopByNameAsync` verwenden den ersten Treffer.
Standardmäßig werden stdout und stderr zeilenweise gepumpt, klassifiziert,
an den optionalen Logger weitergeleitet und über Events veröffentlicht.

## Datenbank und Connection Pool

```text
ConnectionConfiguration
        v
ConnectionFactory -> IDatabaseAdapter -> DbProviderFactories
        v
ConnectionPool / ConnectionManager
```

Die Adapter kennen keine konkreten Provider-Pakete. Die Anwendung liefert den
`ProviderInvariantName` und registriert den Provider selbst. `MinimumPoolSize`
wird validiert, aber der aktuelle Pool erzeugt keine Mindestanzahl von
Verbindungen vorab.

