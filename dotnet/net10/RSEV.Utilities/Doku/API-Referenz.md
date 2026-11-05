# Vollständige API-Referenz

Diese Referenz beschreibt die öffentlichen Typen des aktuell gebauten Pakets
`RSEV.Utilities` (`net8.0`). Private Hilfsmethoden und Felder sind nur dort
erwähnt, wo ihr Verhalten für die Verwendung relevant ist.

## Signatur- und Verhaltenskonventionen

- `Task`-Methoden sind asynchron und sollten mit `await` verwendet werden.
- Nullable-Rückgaben sind mit `?` gekennzeichnet.
- `init`-Properties werden beim Objektinitialisieren gesetzt und danach nicht
  mehr verändert.
- Bei `Get<T>`-Methoden liefern fehlende oder nicht konvertierbare Werte häufig
  `default(T)` statt einer Exception.
- Die API-Referenz folgt dem kompilierten Assemblybestand. Dadurch ist auch
  der aktuell verschachtelte Namespace von `SystemMail` dokumentiert.

## Typen- und Namespace-Index

| Namespace | Öffentliche Typen |
|---|---|
| `RSEV.Utilities.Api` | `ApiRequest`, `ApiResponse`, `HttpApiClient`, `IApiClient`, `RateLimitPolicy`, `RetryPolicy` |
| `RSEV.Utilities.ApiBuilder` | `BaseRuntimeEndpoint`, `EndpointRequest`, `EndpointResponse`, `IRuntimeEndpoint`, `RuntimeApiHost`, `RuntimeEndpointController` |
| `RSEV.Utilities.Commands` | `BaseCommand`, `CommandHandler`, `ICommand` |
| `RSEV.Utilities.Communication` | `MailOptions` |
| `RSEV.Utilities.Communication.RSEV.Utilities.Communication` | `SystemMail` |
| `RSEV.Utilities.Configuration` | `BaseConfig`, `ConfConfig`, `IConfig`, `JsonConfig`, `YamlConfig` |
| `RSEV.Utilities.Connection` | `ConnectionConfiguration`, `ConnectionFactory`, `ConnectionManager`, `ConnectionPool`, `IConnectionManager` |
| `RSEV.Utilities.Controllers` | `BaseController`, `IController` |
| `RSEV.Utilities.Crypto` | `HamacProvider`, `ICryptoProvider`, `KeyManager`, `Sha256Provider` |
| `RSEV.Utilities.Database` | `IDatabaseAdapter`, `MySqlAdapter`, `PostgreSqlAdapter`, `SqliteAdapter`, `SqlServerAdapter` |
| `RSEV.Utilities.Events` | `BaseEventBus`, `BaseEventHandler<T>`, `BaseEventListener<T>`, `EventRegistry<T>`, `IEvent<T>`, `IEventBus`, `IEventHandler<T>`, `IEventListener<T>`, `IEventRegistry<T>` |
| `RSEV.Utilities.Lifecycle` | `IFirstRun`, `IOnStart`, `IOnStop` |
| `RSEV.Utilities.Loading` | `AssemblyLoader`, `TypeDiscovery` |
| `RSEV.Utilities.Logging` | `ILogger`, `SystemLog`, `SystemLog.LogLevel`, `SystemLog.LogColor` |
| `RSEV.Utilities.Messaging` | `BaseMessageBus`, `IMessageBus`, `MessageBusRegistry` |
| `RSEV.Utilities.Networking.Security` | `TlsOptions`, `TlsStreamFactory` |
| `RSEV.Utilities.Plugins` | `GeneratorContext`, `GeneratorPluginLoader`, `GeneratorPluginRegistry`, `IGeneratorPlugin`, `IPlugin`, `PluginRegistry` |
| `RSEV.Utilities.Processes` | `DotNetProcess`, `ExecutableProcess`, `ExecutableResolver`, `IRuntimeSubprocess`, `NodeProcess`, `ProcessLogLevel`, `PythonProcess`, `RuntimeProcessController`, `RuntimeSubprocessBase`, `ShellProcess`, `SubprocessState` |
| `RSEV.Utilities.Runtime` | `BaseRuntimeContext`, `IRuntimeContext`, `ShutdownSummary`, `UnexpectedExitHandler` |
| `RSEV.Utilities.RuntimeClient.Client` | `RuntimeApiClient` |
| `RSEV.Utilities.RuntimeClient.Networking` | `TcpConnection`, `TcpTransport`, `TlsSettings` |
| `RSEV.Utilities.RuntimeClient.Requests` | `RuntimeRequest`, `RuntimeRequestBuilder`, `RuntimeResponse` |
| `RSEV.Utilities.RuntimeClient.Session` | `RuntimeSession` |
| `RSEV.Utilities.Session` | `AccessToken`, `IAccessTokenProvider`, `ISessionManager`, `JwtTokenProvider`, `RefreshToken`, `SessionContext`, `SessionManager` |

---

## Namespace `RSEV.Utilities.Api`

### `ApiRequest`

Eingabeobjekt für `IApiClient` und `HttpApiClient`.

| Member | Typ / Standard | Bedeutung |
|---|---|---|
| `Method` | `HttpMethod`, `GET` | HTTP-Methode |
| `Uri` | `Uri`, `http://localhost` | Zieladresse |
| `Headers` | `IReadOnlyDictionary<string,string>`, leer | Request-Header |
| `Body` | `byte[]`, leer | Rohdaten des Bodys |

Alle Properties sind `init`-Properties.

### `ApiResponse`

Antwortobjekt des unabhängigen `HttpClient`-Moduls.

| Member | Typ | Bedeutung |
|---|---|---|
| `StatusCode` | `int` | numerischer HTTP-Status |
| `Headers` | `IReadOnlyDictionary<string,string>` | Header aus Response- und Content-Headern |
| `Body` | `byte[]` | Rohdaten der Antwort |
| `IsSuccess` | `bool` | `true` bei Status 200 bis 299 |

### `IApiClient`

```csharp
Task<ApiResponse> SendAsync(
    ApiRequest request,
    CancellationToken cancellationToken = default);
```

### `HttpApiClient`

Implementiert `IApiClient` mit einem injizierten `HttpClient`.

```csharp
HttpApiClient(
    HttpClient client,
    RetryPolicy? retryPolicy = null,
    RateLimitPolicy? rateLimitPolicy = null)
Task<ApiResponse> SendAsync(ApiRequest request, CancellationToken cancellationToken = default)
```

Wenn keine Policies übergeben werden, werden Standardinstanzen verwendet.
Jeder Senderversuch prüft zuerst das Rate-Limit. HTTP-Statuscodes ab 500 lösen
die Retry-Policy aus; andere Statuscodes werden direkt zurückgegeben.

### `RetryPolicy`

| Member | Typ / Standard | Bedeutung |
|---|---|---|
| `MaxAttempts` | `int`, `3` | maximale Anzahl Versuche; mindestens 1 wird verwendet |
| `Delay` | `TimeSpan`, 200 ms | Wartezeit zwischen Versuchen |
| `ExecuteAsync<T>` | `Task<T>` | führt eine Operation aus und wiederholt nach `shouldRetry` |

```csharp
Task<T> ExecuteAsync<T>(
    Func<Task<T>> operation,
    Func<T, bool> shouldRetry,
    CancellationToken cancellationToken = default)
```

Exceptions der Operation werden nicht automatisch retried oder umgewandelt.

### `RateLimitPolicy`

| Member | Typ / Standard | Bedeutung |
|---|---|---|
| `RequestsPerWindow` | `int`, `60` | erlaubte Requests im Fenster |
| `Window` | `TimeSpan`, 1 Minute | gleitendes Zeitfenster |
| `TryAcquire()` | `bool` | reserviert einen Request, wenn noch Kapazität vorhanden ist |

Die Reservierung ist thread-safe. Ein überschrittenes Limit liefert `false`;
`HttpApiClient` wirft dann `InvalidOperationException`.

## Namespace `RSEV.Utilities.ApiBuilder`

### `IRuntimeEndpoint`

```csharp
string Route { get; }
Task<EndpointResponse> HandleAsync(EndpointRequest request);
```

Die Route wird vom `RuntimeEndpointController` case-insensitiv verglichen.

### `BaseRuntimeEndpoint`

Abstrakte Basisklasse für Endpoints.

```csharp
protected BaseRuntimeEndpoint(string route)
public virtual string Route { get; protected set; }
public virtual Task<EndpointResponse> HandleAsync(EndpointRequest request)
protected abstract Task<EndpointResponse> OnHandleAsync(EndpointRequest request)
```

`HandleAsync` delegiert an den geschützten Hook `OnHandleAsync`.

### `EndpointRequest`

| Member | Typ / Standard |
|---|---|
| `Route` | `string`, leer |
| `Headers` | `Dictionary<string,string>`, leer |
| `Body` | `byte[]`, leer |

Der aktuelle Typ enthält **kein** `Method`-Property. Der Host liest die
Request-Line zwar ein, verwirft die Methode aber vor dem Erzeugen des Modells.

### `EndpointResponse`

| Member | Typ / Standard |
|---|---|
| `StatusCode` | `int`, 200 |
| `Headers` | `Dictionary<string,string>`, leer |
| `Body` | `byte[]`, leer |

```csharp
static EndpointResponse FromText(
    int statusCode,
    string text,
    string contentType = "text/plain; charset=utf-8")
```

`FromText` kodiert den Text als UTF-8 und setzt `Content-Type`. Eine
`FromJson`-Methode gibt es im aktuellen Build nicht.

### `RuntimeEndpointController`

Thread-safe Registry für `IRuntimeEndpoint`.

| Methode | Rückgabe | Verhalten |
|---|---|---|
| `Register(IRuntimeEndpoint endpoint)` | `bool` | `false`, wenn Route bereits belegt ist |
| `RegisterRange(IEnumerable<IRuntimeEndpoint> endpoints)` | `int` | Anzahl erfolgreicher Registrierungen |
| `Unregister(string route)` | `bool` | entfernt eine Route |
| `Exists(string route)` | `bool` | prüft Route |
| `Resolve(string route)` | `IRuntimeEndpoint?` | liefert Endpoint oder `null` |
| `GetAll()` | `IEnumerable<IRuntimeEndpoint>` | Snapshot der registrierten Endpoints |

### `RuntimeApiHost`

```csharp
RuntimeApiHost(TlsOptions? tlsOptions = null, ILogger? logger = null)
bool IsRunning { get; private set; }
bool Register(IRuntimeEndpoint endpoint)
Task StartAsync(int port, CancellationToken cancellationToken = default)
void Stop()
```

`StartAsync` bindet auf `IPAddress.Any`, nimmt TCP-Verbindungen an und
verarbeitet jede Verbindung separat. Der Host läuft bis zur Cancellation oder
bis `Stop()` aufgerufen wird.

Verarbeitung einer Anfrage:

1. optionaler TLS-Handshake über `TlsStreamFactory`,
2. Request-Line und Header lesen,
3. Body gemäß `Content-Length` lesen,
4. Route auflösen,
5. bei unbekannter Route `404 Not Found`,
6. Endpoint aufrufen,
7. Endpoint-Exception in `500 Internal Server Error` umwandeln,
8. HTTP/1.1-artige Antwort senden.

Der Parser unterstützt kein Chunked Encoding, keine Query-/Routeparameter-
Auflösung und keine vollständige HTTP-Methodenprüfung.

## Namespace `RSEV.Utilities.Commands`

### `ICommand`

| Member | Typ |
|---|---|
| `Name` | `string` |
| `Description` | `string` |
| `ExecuteAsync(IRuntimeContext context, params string[] arguments)` | `Task` |

### `BaseCommand`

```csharp
BaseCommand(string name, string description = "")
public virtual string Name { get; }
public virtual string Description { get; }
public virtual Task ExecuteAsync(IRuntimeContext context, params string[] arguments)
```

Die Standardimplementierung von `ExecuteAsync` erledigt nichts.

### `CommandHandler`

| Methode | Verhalten |
|---|---|
| `Register(ICommand command)` | ignoriert `null`/leeren Namen; gleicher Name überschreibt |
| `RegisterRange(IEnumerable<ICommand> commands)` | registriert alle |
| `Exists(string name)` | case-insensitive Existenzprüfung |
| `Get(string name)` | liefert `ICommand` oder `null` |
| `ExecuteAsync(string name, IRuntimeContext context, params string[] arguments)` | `false` bei unbekanntem Namen, sonst ausführen und `true` |
| `GetAll()` | alle registrierten Commands |

## Namespace `RSEV.Utilities.Communication`

### `MailOptions`

| Property | Typ / Standard | Bedeutung |
|---|---|---|
| `MailFrom` | `string` | Absenderadresse |
| `FromName` | `string` | Anzeigename |
| `MailTo` | `string` | Empfängeradresse |
| `Subject` | `string` | Betreff |
| `MailBody` | `string` | Inhalt |
| `SmtpHost` | `string` | SMTP-Host |
| `SmtpPort` | `int`, 587 | SMTP-Port |
| `SmtpUser` | `string` | SMTP-Benutzer |
| `SmtpPassword` | `string` | SMTP-Passwort |
| `AuthType` | `string`, `Login` | dokumentierte Authentifizierungsart |
| `UseSsl` | `bool`, `false` | SSL aktivieren |
| `UseTls` | `bool`, `true` | TLS aktivieren |
| `MailingMethod` | `string`, `SMTP` | `SMTP` oder lokaler `SendMail`-Pfad |

## Namespace `RSEV.Utilities.Communication`

### `SystemMail`

Der tatsächlich kompilierte Typ liegt aktuell in
`RSEV.Utilities.Communication`.

```csharp
SystemMail()
SystemMail(MailOptions options)
MailOptions Options { get; set; }
void SendMail(string mailFrom, string mailTo, string subject, string body, string fromName = null)
Task SendMailAsync(string mailFrom, string mailTo, string subject, string body, string fromName = null)
void SendMailSmtp(MailOptions options)
Task SendMailSmtpAsync(MailOptions options)
void Send()
Task SendAsync()
```

`SendMail*` ohne `Smtp` verwenden einen SMTP-Client auf `localhost`.
`Send`/`SendAsync` wählen bei `MailingMethod == "SMTP"` den SMTP-Pfad, sonst
den lokalen Pfad. Wenn `Options` `null` ist, sind `Send` und `SendAsync`
No-ops.

## Namespace `RSEV.Utilities.Configuration`

### `IConfig`

```csharp
void Load(string path)
Task LoadAsync(string path)
void Save(string path)
Task SaveAsync(string path)
T Get<T>(string key)
void Set<T>(string key, T value)
```

### `BaseConfig`

Formatneutrale Basisklasse mit internem `Dictionary<string, object>`.

| Methode | Standardverhalten |
|---|---|
| `Load` / `Save` | leer; für Unterklassen vorgesehen |
| `LoadAsync` / `SaveAsync` | ruft die synchrone Variante auf und liefert abgeschlossenen Task |
| `Get<T>` | gibt gespeicherten Wert bei passendem Typ zurück, sonst `default` |
| `Set<T>` | speichert oder überschreibt Wert |

### `ConfConfig`

Überschreibt alle Dateioperationen für `key=value`. Leere Zeilen und Zeilen,
die nach Trim mit `#` beginnen, werden ignoriert. `Get<T>` nutzt
`Convert.ChangeType`; Konvertierungsfehler liefern `default`.

### `JsonConfig`

Überschreibt alle Dateioperationen mit `System.Text.Json` und eingerückter
Ausgabe. `SerializerOptions` ist als `protected virtual` verfügbar und kann
in einer Unterklasse angepasst werden. Beim Laden werden JSON-Werte als
`JsonElement` gespeichert und in `Get<T>` erneut deserialisiert.

### `YamlConfig`

Unterstützt die einfache, interne YAML-Teilmenge:

- `key: value`,
- Kommentare mit `#`,
- verschachtelte Schlüssel über Einrückung,
- Listenzeilen mit `- `.

`ParseYaml(string[] lines)` ist `protected virtual` und erweiterbar. Das
Speichern schreibt flache Zeilen und ist kein vollständiger YAML-Serializer.

## Namespace `RSEV.Utilities.Connection`

### `ConnectionConfiguration`

| Property | Typ / Standard |
|---|---|
| `ProviderInvariantName` | `string`, leer |
| `ConnectionString` | `string`, leer |
| `MinimumPoolSize` | `int`, 1 |
| `MaximumPoolSize` | `int`, 10 |

```csharp
void Validate()
```

Validiert Providername, Connection String und Poolgrenzen. `MinimumPoolSize`
muss mindestens 0 sein, `MaximumPoolSize` mindestens 1 und Minimum darf
Maximum nicht überschreiten.

### `ConnectionFactory`

```csharp
ConnectionFactory(IDatabaseAdapter adapter)
DbConnection Create(ConnectionConfiguration configuration)
```

`Create` validiert die Konfiguration und delegiert an den Adapter.

### `ConnectionPool`

```csharp
ConnectionPool(int maximumSize = 10)
DbConnection Rent(Func<DbConnection> factory)
bool Return(DbConnection connection)
void Clear()
void Dispose()
```

`Rent` nimmt eine Verbindung aus dem Pool oder erstellt sie über `factory`.
`Return` legt sie bis zur Maximalgröße zurück; überschüssige Verbindungen
werden disposed und liefern `false`. Der Pool prüft die Gesundheit der
Verbindung nicht.

### `IConnectionManager`

```csharp
DbConnection Open(ConnectionConfiguration configuration)
void Return(DbConnection connection)
void Dispose()
```

### `ConnectionManager`

```csharp
ConnectionManager(ConnectionFactory factory, ConnectionConfiguration configuration)
DbConnection Open(ConnectionConfiguration configuration)
void Return(DbConnection connection)
void Dispose()
```

Der Manager erzeugt einen `ConnectionPool` mit `MaximumPoolSize` aus dem
Konstruktor-Setup. `Open` verwendet die übergebene Konfiguration für die
Erzeugung einer neuen Verbindung.

## Namespace `RSEV.Utilities.Database`

### `IDatabaseAdapter`

```csharp
string Provider { get; }
DbConnection CreateConnection(ConnectionConfiguration configuration)
```

### Provideradapter

`MySqlAdapter`, `SqlServerAdapter`, `PostgreSqlAdapter` und `SqliteAdapter`
implementieren jeweils `IDatabaseAdapter`:

| Klasse | `Provider`-Wert | Verhalten |
|---|---|---|
| `MySqlAdapter` | `mysql` | `DbProviderFactories.GetFactory(...)` |
| `SqlServerAdapter` | `mssql` | `DbProviderFactories.GetFactory(...)` |
| `PostgreSqlAdapter` | `postgresql` | `DbProviderFactories.GetFactory(...)` |
| `SqliteAdapter` | `sqlite` | `DbProviderFactories.GetFactory(...)` |

Alle besitzen einen parameterlosen Konstruktor und setzen den erstellten
`DbConnection` auf den konfigurierten `ConnectionString`. Ein konkreter
ADO.NET-Provider muss vorher registriert sein.

## Namespace `RSEV.Utilities.Controllers`

### `IController`

```csharp
string Name { get; }
void Initialize(IRuntimeContext context)
Task InitializeAsync(IRuntimeContext context)
void Shutdown(IRuntimeContext context)
Task ShutdownAsync(IRuntimeContext context)
```

### `BaseController`

```csharp
protected BaseController(string name)
public virtual string Name { get; }
public virtual void Initialize(IRuntimeContext context)
public virtual Task InitializeAsync(IRuntimeContext context)
public virtual void Shutdown(IRuntimeContext context)
public virtual Task ShutdownAsync(IRuntimeContext context)
```

Erweiterungspunkte sind `OnInitialize`, `OnInitializeAsync`, `OnShutdown` und
`OnShutdownAsync`. Die Standard-Hooks erledigen nichts.

## Namespace `RSEV.Utilities.Crypto`

### `ICryptoProvider`

```csharp
string Algorithm { get; }
byte[] Compute(byte[] data)
bool Verify(byte[] data, byte[] signature)
```

### `HamacProvider`

```csharp
HamacProvider(byte[] key)
string Algorithm // "HAMAC-SHA256"
byte[] Compute(byte[] data)
bool Verify(byte[] data, byte[] signature)
```

Der Name `HamacProvider` ist Bestandteil der bestehenden API; intern wird
HMAC-SHA-256 verwendet. Der Schlüssel wird beim Konstruktor kopiert und darf
nicht leer sein. `Verify` verwendet einen konstantzeitnahen Vergleich.

### `Sha256Provider`

```csharp
Sha256Provider()
string Algorithm // "SHA-256"
byte[] Compute(byte[] data)
bool Verify(byte[] data, byte[] signature)
```

`Verify` vergleicht die SHA-256-Prüfsumme mit `signature`. Dieser Provider ist
nicht geheimnisbasiert.

### `KeyManager`

```csharp
KeyManager()
byte[] Generate(int size = 32)
```

`size < 16` löst `ArgumentOutOfRangeException` aus.

## Namespace `RSEV.Utilities.Events`

Alle generischen Eventtypen verlangen `TEventArgs : EventArgs`.

### `IEvent<TEventArgs>`

| Member | Typ |
|---|---|
| `Name` | `string` |
| `Description` | `string` |
| `EventArgsType` | `Type`, Defaultimplementierung `typeof(TEventArgs)` |

### `IEventHandler<TEventArgs>` und `IEventListener<TEventArgs>`

```csharp
// Handler
void Handle(object sender, TEventArgs args)
Task HandleAsync(object sender, TEventArgs args)

// Listener
void Listen(object sender, TEventArgs args)
Task ListenAsync(object sender, TEventArgs args)
```

### `BaseEventHandler<TEventArgs>`

Implementiert die beiden Handlermethoden und delegiert an die geschützten
Hooks `OnHandle` und `OnHandleAsync`. Die Standard-Hooks sind No-ops.

### `BaseEventListener<TEventArgs>`

Implementiert `Listen` und `ListenAsync` und delegiert an `OnListen` und
`OnListenAsync`. Die Standard-Hooks sind No-ops.

### `IEventRegistry<TEventArgs>`

```csharp
string EventName { get; }
void Subscribe(IEventHandler<TEventArgs> handler)
void Unsubscribe(IEventHandler<TEventArgs> handler)
void Subscribe(IEventListener<TEventArgs> listener)
void Unsubscribe(IEventListener<TEventArgs> listener)
IReadOnlyCollection<IEventHandler<TEventArgs>> GetHandlers()
IReadOnlyCollection<IEventListener<TEventArgs>> GetListeners()
void Publish(object sender, TEventArgs args)
Task PublishAsync(object sender, TEventArgs args)
```

### `EventRegistry<TEventArgs>`

```csharp
EventRegistry(string eventName)
string EventName { get; }
```

Implementiert alle Methoden von `IEventRegistry<TEventArgs>`. Leere Eventnamen
werfen beim Konstruktor `ArgumentException`. Doppelte Handler oder Listener
werden nicht zweimal eingetragen. Die Registry verwendet Locks für die
Sammlungen und liefert bei `GetHandlers`/`GetListeners` Snapshots.

Beim synchronen und asynchronen Publizieren werden Ausnahmen einzelner
Empfänger nicht nach außen weitergegeben.

### `IEventBus`

Erbt von `IMessageBus` und ergänzt:

```csharp
void RegisterEventRegistry<TEventArgs>(IEventRegistry<TEventArgs> registry)
    where TEventArgs : EventArgs
IEventRegistry<TEventArgs> GetRegistry<TEventArgs>(string eventName)
    where TEventArgs : EventArgs
bool HasRegistry<TEventArgs>(string eventName)
    where TEventArgs : EventArgs
IEnumerable<string> GetAllEventNames()
```

### `BaseEventBus`

Erbt von `BaseMessageBus` und implementiert `IEventBus`.

```csharp
protected BaseEventBus(string name, string description = null)
```

Registries werden intern anhand von Eventname und vollständigem Argumenttyp
unterschieden. Ein späteres `RegisterEventRegistry` mit demselben Schlüssel
ersetzt die bisherige Registry.

## Namespace `RSEV.Utilities.Loading`

### `AssemblyLoader`

| Methode | Verhalten |
|---|---|
| `LoadAssembly(string path)` | lädt eine DLL mit `Assembly.LoadFrom`; bei Fehler `null` |
| `LoadAssembliesFromDirectory(string directory)` | lädt alle `*.dll` im direkten Verzeichnis |
| `FindTypesImplementing<T>(Assembly assembly)` | konkrete, nicht abstrakte Klassen, die `T` implementieren/erben |
| `FindTypesImplementing<T>(IEnumerable<Assembly> assemblies)` | Aggregation über mehrere Assemblies |
| `CreateInstance<T>(Type type)` | parameterlosen Konstruktor verwenden; bei Fehler `default` |
| `CreateInstancesImplementing<T>(Assembly assembly)` | passende Typen finden und instanziieren |
| `CreateInstancesImplementing<T>(IEnumerable<Assembly> assemblies)` | Aggregation über mehrere Assemblies |

### `TypeDiscovery`

| Methode | Verhalten |
|---|---|
| `FindTypesImplementing<T>(Assembly assembly)` | konkrete, nicht abstrakte Klassen |
| `FindTypesImplementing<T>(IEnumerable<Assembly> assemblies)` | Aggregation |
| `FindTypesWithAttribute<TAttribute>(Assembly assembly)` | konkrete Klassen mit Attribut |
| `FindTypesWithAttribute<TAttribute>(IEnumerable<Assembly> assemblies)` | Aggregation |
| `FindTypesByPredicate(Assembly assembly, Func<Type,bool> predicate)` | konkrete Klassen gemäß Prädikat |
| `FindTypesByPredicate(IEnumerable<Assembly> assemblies, Func<Type,bool> predicate)` | Aggregation |

## Namespace `RSEV.Utilities.Logging`

### `ILogger`

```csharp
void Log(SystemLog.LogLevel level, string message)
void LogDebug(string message)
void LogInfo(string message)
void LogWarning(string message)
void LogError(string message)
void LogCritical(string message)
```

### `SystemLog`

```csharp
SystemLog(string? customLogDirectory = null)
void Init()
void Close()
void LogDebug(string message)
void LogInfo(string message)
void LogWarning(string message)
void LogError(string message)
void LogCritical(string message)
void Log(string source, string message, LogLevel level)
void Log(LogLevel level, string message)
void Log(LogLevel level, string message, LogColor? overrideColor = null)
```

`Init` legt das Logverzeichnis an, rotiert eine vorhandene `latest.log` und
öffnet eine neue Datei. `Close` schließt und archiviert die aktuelle Datei.
Alle Methoden sind `virtual`.

### `SystemLog.LogLevel`

| Wert | Numerischer Wert |
|---|---:|
| `Debug` | 0 |
| `Info` | 1 |
| `Warnung` | 2 |
| `Error` | 3 |
| `Kritisch` | 4 |

### `SystemLog.LogColor`

`Gray`, `Green`, `Yellow`, `Red`, `Cyan`, `Magenta`, `White`.

Die Farbe wird nur auf die Console-Ausgabe angewendet. `LogLevelNames` sowie
die Mapping-Methoden sind geschützte virtuelle Erweiterungspunkte.

## Namespace `RSEV.Utilities.Lifecycle`

### `IOnStart`

```csharp
void OnStart(IRuntimeContext context)
Task OnStartAsync(IRuntimeContext context)
```

### `IOnStop`

```csharp
void OnStop(IRuntimeContext context)
Task OnStopAsync(IRuntimeContext context)
```

### `IFirstRun`

Erbt von `IOnStart` und ergänzt:

```csharp
void OnFirstRun(IRuntimeContext context)
Task OnFirstRunAsync(IRuntimeContext context)
```

Das Framework speichert nicht automatisch, ob ein First Run bereits statt-
gefunden hat; die Anwendung muss diesen Zustand verwalten.

## Namespace `RSEV.Utilities.Messaging`

### `IMessageBus`

```csharp
string Name { get; }
string Description { get; }
void Initialize(IRuntimeContext context)
Task InitializeAsync(IRuntimeContext context)
void Shutdown(IRuntimeContext context)
Task ShutdownAsync(IRuntimeContext context)
```

### `BaseMessageBus`

```csharp
protected BaseMessageBus(string name, string description = null)
public virtual string Name { get; }
public virtual string Description { get; }
public virtual void Initialize(IRuntimeContext context)
public virtual Task InitializeAsync(IRuntimeContext context)
public virtual void Shutdown(IRuntimeContext context)
public virtual Task ShutdownAsync(IRuntimeContext context)
```

Geschützte Hooks sind `OnInitialize`, `OnInitializeAsync`, `OnShutdown` und
`OnShutdownAsync`. Die Standardimplementierungen sind No-ops.

### `MessageBusRegistry`

| Methode | Verhalten |
|---|---|
| `Register(IMessageBus bus)` | ignoriert `null`/leeren Namen; gleicher Name ersetzt |
| `RegisterRange(IEnumerable<IMessageBus> buses)` | registriert alle |
| `Exists(string name)` | case-insensitive Existenzprüfung |
| `Get(string name)` | Bus oder `null` |
| `GetAll()` | alle registrierten Busse |
| `LoadMessageBuses(string directory, IRuntimeContext context)` | DLLs laden, Busse finden, registrieren und synchron initialisieren |
| `LoadMessageBusesAsync(string directory, IRuntimeContext context)` | asynchron initialisieren |
| `ShutdownAll(IRuntimeContext context)` | alle synchron beenden |
| `ShutdownAllAsync(IRuntimeContext context)` | alle asynchron beenden |

Discovery und Instanziierung erfordern konkrete, parameterlose Busklassen.

## Namespace `RSEV.Utilities.Networking.Security`

### `TlsOptions`

| Property | Typ / Standard | Bedeutung |
|---|---|---|
| `Enabled` | `bool`, `false` | TLS aktivieren |
| `ServerCertificate` | `X509Certificate2?` | Serverzertifikat |
| `RequireClientCertificate` | `bool`, `false` | Clientzertifikat anfordern |
| `EnabledProtocols` | `SslProtocols`, TLS 1.2 + TLS 1.3 | zulässige Protokolle |
| `AllowInvalidCertificates` | `bool`, `false` | Client-Zertifikatsfehler ignorieren |
| `TargetHost` | `string?` | Zielhost für Client-SNI/Validierung |

Statische Fabriken:

```csharp
static TlsOptions None { get; }
static TlsOptions ForServer(X509Certificate2 certificate)
static TlsOptions ForClient(string targetHost)
```

### `TlsStreamFactory`

```csharp
static Task<Stream> CreateServerStreamAsync(
    TcpClient client,
    TlsOptions options,
    CancellationToken cancellationToken = default)

static Task<Stream> CreateClientStreamAsync(
    TcpClient client,
    TlsOptions options,
    CancellationToken cancellationToken = default)
```

Bei deaktiviertem oder `null`-ähnlichem Optionswert wird der rohe
`NetworkStream` zurückgegeben. Serverseitig ist bei aktiviertem TLS ein
`ServerCertificate` erforderlich; clientseitig ein `TargetHost`.

## Namespace `RSEV.Utilities.Plugins`

### `IPlugin`

```csharp
string Name { get; }
string Description { get; }
void OnEnable(IRuntimeContext context)
Task OnEnableAsync(IRuntimeContext context)
void OnDisable(IRuntimeContext context)
Task OnDisableAsync(IRuntimeContext context)
void OnReload(IRuntimeContext context)
Task OnReloadAsync(IRuntimeContext context)
```

### `PluginRegistry`

| Methode | Verhalten |
|---|---|
| `LoadPlugins(string directory, IRuntimeContext context)` | lädt DLLs und aktiviert gefundene Plugins synchron |
| `LoadPluginsAsync(string directory, IRuntimeContext context)` | lädt und aktiviert asynchron |
| `DisableAll(IRuntimeContext context)` | deaktiviert alle geladenen Plugins |
| `DisableAllAsync(IRuntimeContext context)` | asynchron deaktivieren |
| `EnableAll(IRuntimeContext context)` | alle erneut aktivieren |
| `EnableAllAsync(IRuntimeContext context)` | asynchron erneut aktivieren |
| `ForceReload(string directory, IRuntimeContext context)` | deaktivieren, Liste leeren, Verzeichnis neu laden |
| `ForceReloadAsync(string directory, IRuntimeContext context)` | asynchrone Variante |

Das Registry-Objekt bietet aktuell keine öffentliche `GetAll`- oder
`Unregister`-Methode. Plugins müssen einen parameterlosen Konstruktor besitzen.

### `IGeneratorPlugin`

```csharp
string Name { get; }
Version Version { get; }
void Generate(GeneratorContext context)
```

### `GeneratorContext`

```csharp
GeneratorContext(IDictionary<string, object?>? values = null)
IReadOnlyDictionary<string, object?> Values { get; }
```

### `GeneratorPluginRegistry`

```csharp
void Register(IGeneratorPlugin plugin)
IReadOnlyList<IGeneratorPlugin> All()
void Generate(GeneratorContext context)
```

Die Registrierung ist nach Pluginname eindeutig und case-sensitiv. Ein
Duplikat führt zu `InvalidOperationException`. `All()` liefert nach Namen
sortierte Plugins.

### `GeneratorPluginLoader`

```csharp
int Discover(string directory, GeneratorPluginRegistry registry)
```

Durchsucht nur DLLs direkt im angegebenen Verzeichnis, instanziiert konkrete
`IGeneratorPlugin`-Typen und gibt die Anzahl der gefundenen/registrierten
Instanzen zurück.

## Namespace `RSEV.Utilities.Processes`

### `SubprocessState`

`Created`, `Starting`, `Running`, `Stopping`, `Stopped`, `Faulted`.

### `ProcessLogLevel`

`Info`, `Warning`, `Error`.

### `IRuntimeSubprocess`

| Member | Typ |
|---|---|
| `ProcessId` | `string` |
| `Name` | `string` |
| `Executable` | `string` |
| `Arguments` | `string[]` |
| `State` | `SubprocessState` |
| `ExitCode` | `int?` |
| `Properties` | `IDictionary<string, object>` |
| `OnStdOut` | `event Action<IRuntimeSubprocess,string>` |
| `OnStdErr` | `event Action<IRuntimeSubprocess,string>` |

Methoden:

```csharp
Task<bool> StartAsync(CancellationToken cancellationToken = default)
Task<bool> StopAsync()
Task<bool> RestartAsync(CancellationToken cancellationToken = default)
Task<bool?> CheckHealthAsync()
```

### `RuntimeSubprocessBase`

```csharp
protected RuntimeSubprocessBase(string name, ILogger? logger = null)
string ProcessId { get; }
string Name { get; }
abstract string Executable { get; }
abstract string[] Arguments { get; }
SubprocessState State { get; protected set; }
int? ExitCode { get; }
IDictionary<string, object> Properties { get; }
```

Die Prozess-ID ist eine gekürzte GUID. Die abstrakten Hooks für eigene
Adapter sind:

```csharp
protected abstract Task<bool> OnStartAsync(CancellationToken cancellationToken)
protected abstract Task<bool> OnStopAsync()
protected abstract ProcessLogLevel ClassifyLogLine(string line)
```

Zusätzlich sind die geschützten Hilfen `LogInfo`, `LogWarning`, `LogError`,
`LogByLevel`, `RaiseStdOut` und `RaiseStdErr` verfügbar. Der Standard-
Healthcheck meldet `true` genau bei `Running`.

### Konkrete Prozessadapter

| Klasse | Konstruktor | `Executable` |
|---|---|---|
| `DotNetProcess` | `(string name, string dllPath, string[]? arguments = null, ILogger? logger = null)` | `dotnet` |
| `NodeProcess` | `(string name, string scriptPath, string[]? arguments = null, ILogger? logger = null)` | `node` |
| `PythonProcess` | `(string name, string scriptPath, string[]? arguments = null, ILogger? logger = null)` | `python` |
| `ShellProcess` | `(string name, string command, ILogger? logger = null)` | Windows: `pwsh`; sonst `/bin/bash` |
| `ExecutableProcess` | `(string name, string executablePath, string[]? arguments = null, ILogger? logger = null)` | übergebener Pfad |
| `ExecutableProcess` | `(string name, string environmentVariable, string windowsPath, string linuxPath, string[]? arguments = null, ILogger? logger = null)` | Ergebnis von `ExecutableResolver.Resolve` |

Alle konkreten Adapter überschreiben `Arguments`. Sie starten mit
`UseShellExecute = false`, leiten stdout/stderr um und beenden den Prozess bei
`StopAsync` rekursiv (`Kill(true)`). Die Startlogik wartet rund 500 ms; Prozesse,
die bis dahin beendet sind, werden bei den meisten Adaptern als fehlgeschlagen
gewertet. `ShellProcess` behandelt ein bereits beendetes Kommando als Erfolg.

### `ExecutableResolver`

Statische Hilfsmethoden:

```csharp
static string Resolve(string environmentVariable, string windowsPath, string linuxPath)
static void EnsureExecutablePermissions(string executablePath)
```

`Resolve` prüft zuerst die angegebene Umgebungsvariable. Unter Windows werden
mehrere relative Pfade geprüft. Unter Linux wird ein absoluter vorhandener
Pfad zurückgegeben; ein anderer Wert wird als Systemkommando zurückgegeben.
`EnsureExecutablePermissions` setzt unter Linux mit `chmod +x` die
Ausführungsrechte, falls die Datei vorhanden ist.

### `RuntimeProcessController`

| Methode | Rückgabe / Verhalten |
|---|---|
| `Register(IRuntimeSubprocess process)` | `bool`; Registrierung nach `ProcessId` |
| `RegisterRange(IEnumerable<IRuntimeSubprocess> processes)` | Anzahl erfolgreicher Registrierungen |
| `Exists(string processId)` | `bool` |
| `Get(string processId)` | Prozess oder `null` |
| `GetAll()` | Snapshot aller Prozesse |
| `FindByName(string name)` | case-insensitive Trefferliste |
| `FindByState(SubprocessState state)` | Trefferliste nach Zustand |
| `Unregister(string processId)` | `bool`; entfernt nur aus Registry, stoppt nicht |
| `StartAsync(string processId, CancellationToken token = default)` | startet Prozess oder `false` bei unbekannter ID |
| `StopAsync(string processId)` | stoppt Prozess oder `false` bei unbekannter ID |
| `StartByNameAsync(string name, CancellationToken token = default)` | startet ersten Namenstreffer |
| `StopByNameAsync(string name)` | stoppt ersten Namenstreffer |
| `StartAllAsync(CancellationToken token = default)` | Anzahl erfolgreicher Starts |
| `StopAllAsync()` | Anzahl erfolgreicher Stops |
| `CheckAllHealthAsync()` | `IDictionary<string,bool?>` nach ProcessId |
| `ListProcesses()` | Tupel `(ProcessId, Name, State)` |

Der Konstruktor ist `RuntimeProcessController(ILogger? logger = null)`.

## Namespace `RSEV.Utilities.Runtime`

### `IRuntimeContext`

| Property | Typ |
|---|---|
| `Config` | `IConfig` |
| `Logger` | `ILogger` |
| `AssemblyLoader` | `AssemblyLoader` |
| `TypeDiscovery` | `TypeDiscovery` |
| `PluginRegistry` | `PluginRegistry` |
| `MessageBusRegistry` | `MessageBusRegistry` |
| `ProcessController` | `RuntimeProcessController` |
| `ApiHost` | `RuntimeApiHost` |
| `GlobalState` | `IDictionary<string, object>` |
| `IsRunning` | `bool` get/set |

```csharp
void Set(string key, object value)
T Get<T>(string key)
bool TryGet<T>(string key, out T value)
```

### `BaseRuntimeContext`

Abstrakte Standardimplementierung von `IRuntimeContext`.

```csharp
protected BaseRuntimeContext()
```

Öffentliche Properties und Methoden entsprechen `IRuntimeContext`. Die
folgenden Factory-Methoden sind `protected virtual` und können überschrieben
werden:

```csharp
protected virtual ILogger CreateLogger()
protected virtual IConfig CreateDefaultConfig()
protected virtual AssemblyLoader CreateAssemblyLoader()
protected virtual TypeDiscovery CreateTypeDiscovery()
protected virtual PluginRegistry CreatePluginRegistry()
protected virtual MessageBusRegistry CreateMessageBusRegistry()
protected virtual RuntimeProcessController CreateProcessController()
protected virtual RuntimeApiHost CreateApiHost()
```

Der Standard verwendet `ConfConfig`, `AssemblyLoader`, `TypeDiscovery`,
`PluginRegistry`, `MessageBusRegistry`, `RuntimeProcessController`,
`SystemLog`, `RuntimeApiHost` und ein neues Dictionary für `GlobalState`.

### `ShutdownSummary`

| Member | Typ | Bedeutung |
|---|---|---|
| `StartTime` | `DateTime` | UTC-Startzeitpunkt |
| `EndTime` | `DateTime` | UTC-Endzeitpunkt nach `Complete()` |
| `SuccessfulComponents` | `List<string>` | erfolgreich beendete Komponenten |
| `Errors` | `List<string>` | Fehlertexte |

```csharp
ShutdownSummary()
void AddSuccess(string componentName)
void AddError(string error)
void Complete()
override string ToString()
```

### `UnexpectedExitHandler`

```csharp
UnexpectedExitHandler(IRuntimeContext context)
ShutdownSummary Handle(Exception exception)
Task<ShutdownSummary> HandleAsync(Exception exception)
```

Der Handler setzt `context.IsRunning = false`, deaktiviert die Plugins
synchron oder asynchron und sammelt Fehler in einer `ShutdownSummary`. Andere
Subsysteme werden nicht automatisch beendet.

## Namespace `RSEV.Utilities.RuntimeClient.Requests`

### `RuntimeRequest`

| Property | Typ / Standard | Bedeutung |
|---|---|---|
| `Route` | `string`, leer | Zielroute |
| `Payload` | `object?` | serialisierbare Nutzlast |
| `Headers` | `Dictionary<string,string>`, leer | Request-Header |
| `SessionId` | `string?` | gebundene Session |

### `RuntimeRequestBuilder`

Fluenter Builder mit Standardroute `/`.

```csharp
RuntimeRequestBuilder WithRoute(string route)
RuntimeRequestBuilder WithPayload(object payload)
RuntimeRequestBuilder WithHeader(string key, string value)
RuntimeRequestBuilder BindSession(RuntimeSession session)
RuntimeRequest Build()
byte[] BuildBytes()
```

`Build` erzeugt eine Kopie der Header und hängt eine Session-ID an, falls eine
Session gebunden wurde. `BuildBytes` serialisiert eine vorhandene Payload mit
`System.Text.Json`, setzt `Content-Type: application/json; charset=utf-8`,
`X-Session-Id` und `Content-Length` und erzeugt immer eine `POST`-Request-Line.

### `RuntimeResponse`

| Property | Typ / Standard |
|---|---|
| `StatusCode` | `int` |
| `Body` | `byte[]`, leer |
| `Headers` | `Dictionary<string,string>`, leer |
| `IsError` | `bool`, `StatusCode >= 400` |
| `ErrorMessage` | `string?` |

```csharp
string GetBodyAsText()
```

`GetBodyAsText` dekodiert den Body als UTF-8.

## Namespace `RSEV.Utilities.RuntimeClient.Session`

### `RuntimeSession`

```csharp
RuntimeSession(string sessionId, DateTime expiresAt)
string SessionId { get; set; }
DateTime ExpiresAt { get; set; }
Dictionary<string, object> Metadata { get; set; }
bool IsExpired { get; }
void AttachToRequest(RuntimeRequest request)
Task RenewAsync()
```

Die Standardimplementierung von `RenewAsync` verlängert `ExpiresAt` lokal um
eine Stunde. Für eine echte Servererneuerung muss die Methode überschrieben
werden.

## Namespace `RSEV.Utilities.RuntimeClient.Networking`

### `TlsSettings`

| Property | Typ / Standard |
|---|---|
| `ClientCertificate` | `X509Certificate2?` |
| `ValidateServerCertificate` | `bool`, `true` |
| `AllowedProtocols` | `SslProtocols`, TLS 1.2 + TLS 1.3 |

```csharp
TlsOptions ToTlsOptions(string targetHost)
```

Die Methode setzt `Enabled`, `TargetHost`, `EnabledProtocols` und die Negation
von `ValidateServerCertificate` als `AllowInvalidCertificates`. Die Property
`ClientCertificate` wird aktuell nicht übernommen.

### `TcpConnection`

```csharp
TcpConnection(TcpClient client, Stream stream, bool isTls)
TcpClient Client { get; }
Stream Stream { get; }
bool IsTls { get; }
void Close()
```

`Close` disposed Stream und `TcpClient`.

### `TcpTransport`

```csharp
TcpTransport()
Task<TcpConnection> ConnectAsync(
    string host,
    int port,
    bool useTls,
    TlsSettings? tlsSettings = null,
    CancellationToken cancellationToken = default)
Task SendAsync(byte[] data, CancellationToken cancellationToken = default)
Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default)
void Close()
```

`SendAsync` und `ReceiveAsync` werfen `InvalidOperationException`, wenn vorher
kein `ConnectAsync` erfolgreich war. `ReceiveAsync` liest verfügbare Daten und
wartet nicht zwingend auf ein Protokollende oder EOF.

## Namespace `RSEV.Utilities.RuntimeClient.Client`

### `RuntimeApiClient`

```csharp
RuntimeApiClient(
    string host,
    int port,
    bool useTls = false,
    TlsSettings? tlsSettings = null)
void SetSession(RuntimeSession session)
Task<RuntimeResponse> CallAsync(
    string route,
    object payload,
    CancellationToken cancellationToken = default)
Task<RuntimeResponse> SendAsync(
    RuntimeRequest request,
    CancellationToken cancellationToken = default)
```

Vor jedem Aufruf wird eine abgelaufene gebundene `RuntimeSession` über
`RenewAsync` erneuert. Für `SendAsync` werden Route, Payload und Header in
einen neuen Builder kopiert; die Session des Clients wird dabei bevorzugt
gebunden. Jede Anfrage öffnet eine TCP-/TLS-Verbindung und schließt sie im
`finally`-Block.

Die Response-Auswertung erwartet eine HTTP-artige Status-Line und den Header-
Body-Trenner `\r\n\r\n`. Eine nicht parsbare Antwort wird zu Status `502`.

## Namespace `RSEV.Utilities.Session`

### `AccessToken` und `RefreshToken`

Beide Typen besitzen:

| Property | Typ |
|---|---|
| `Value` | `string`, init |
| `ExpiresAt` | `DateTimeOffset`, init |
| `IsExpired` | `bool`, UTC-Vergleich |

### `SessionContext`

| Property | Typ |
|---|---|
| `Subject` | `string`, init |
| `AccessToken` | `AccessToken?`, init |
| `RefreshToken` | `RefreshToken?`, init |

### `IAccessTokenProvider`

```csharp
AccessToken Create(string subject, TimeSpan lifetime)
bool Validate(AccessToken token)
```

### `JwtTokenProvider`

```csharp
JwtTokenProvider(byte[] secret)
AccessToken Create(string subject, TimeSpan lifetime)
bool Validate(AccessToken token)
```

Erzeugt und signiert JWTs mit HS256. Der Secret wird kopiert und darf nicht
leer sein. Leerer Subject führt bei `Create` zu `ArgumentException`.

### `ISessionManager`

```csharp
SessionContext Create(string subject)
bool IsValid(SessionContext context)
```

### `SessionManager`

```csharp
SessionManager(
    IAccessTokenProvider tokenProvider,
    TimeSpan? accessTokenLifetime = null,
    TimeSpan? refreshTokenLifetime = null)
SessionContext Create(string subject)
bool IsValid(SessionContext context)
```

Standards sind eine Access-Token-Lebensdauer von einer Stunde und eine
Refresh-Token-Lebensdauer von 30 Tagen. `IsValid` verlangt einen gültigen
Access-Token und einen nicht abgelaufenen Refresh-Token.





