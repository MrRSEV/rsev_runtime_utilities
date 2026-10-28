====== RSEV.Utilities .NET Wiki ======

Dieses Wiki beschreibt die .NET-Implementierung von RSEV.Utilities. Ziel-
Framework ist net8.0; alle beschriebenen neuen Module verwenden ausschließlich
System.*-APIs. Es gibt keine externen NuGet-Pakete.

===== Dateien im Wiki =====

  * README.txt       Einstieg, Konzepte und Beispiele
  * API-Referenz.txt Vollständige Klassen-/Methodenübersicht

===== 1. Installation und Einstieg =====

    dotnet build dotnet/net10/RSEV.Utilities/RSEV.Utilities.csproj

Beispiel für einen Runtime-Kontext:

    using RSEV.Utilities.Runtime;
    using RSEV.Utilities.Logging;

    var log = new SystemLog("./log");
    log.Init();
    var context = new BaseRuntimeContext(log);
    context.SetRunning(true);
    log.LogInfo("RSEV started");

BaseRuntimeContext bündelt Config, AssemblyLoader, TypeDiscovery, Plugins,
MessageBus, Prozesse, API-Host, Logger und GlobalState.

===== 2. Architektur =====

    Runtime / Configuration / Logging
                |
    API Builder / RuntimeClient / Networking
                |
    Database -> Connection -> Crypto -> Session
                |
       Events / Messaging / Commands / Plugins

ApiBuilder ist der leichte TCP-/HTTP-Host. RuntimeClient ist der Client mit
RequestBuilder, Sessionbindung und TCP/TLS-Transport. Die neuen API-Tools im
Namespace RSEV.Utilities.Api sind ein unabhängiger HttpClient-Vertrag.

===== 3. Konfiguration, DI und Loading =====

IConfig und BaseConfig bilden die Konfigurationsbasis. JsonConfig, YamlConfig
und ConfConfig laden bzw. speichern Werte synchron oder asynchron mit
System.Text.Json bzw. den vorhandenen Formatabstraktionen.

    var config = new JsonConfig();
    config.Set("environment", "dev");
    await config.SaveAsync("app.json");
    await config.LoadAsync("app.json");
    var environment = config.Get<string>("environment");

AssemblyLoader lädt Assemblies aus Verzeichnissen. TypeDiscovery findet Typen,
die ein Interface oder eine Basisklasse implementieren. Diese Klassen bilden
die Grundlage für Plugin- und Service-Discovery.

===== 4. Logging =====

    var logger = new SystemLog("./log");
    logger.Init();
    logger.LogDebug("debug");
    logger.LogInfo("started");
    logger.LogWarning("slow request");
    logger.LogError("request failed");
    logger.LogCritical("shutdown required");
    logger.Close();

SystemLog unterstützt LogLevel Debug, Info, Warnung, Error und Kritisch,
LogColor, Konsolenausgabe, Dateiausgabe und Logrotation. ILogger ist der
injizierbare Vertrag. RuntimeProcessController akzeptiert einen optionalen
ILogger und protokolliert Lifecycle-Ereignisse.

===== 5. Database und Connection Manager =====

IDatabaseAdapter sowie MySqlAdapter, SqlServerAdapter, PostgreSqlAdapter und
SqliteAdapter nutzen DbProviderFactories. Damit bleibt das Framework frei von
konkreten Datenbankpaketen; der Provider wird durch die Anwendung registriert.

    using RSEV.Utilities.Connection;
    using RSEV.Utilities.Database;

    var configuration = new ConnectionConfiguration {
        ProviderInvariantName = "Microsoft.Data.Sqlite",
        ConnectionString = "Data Source=app.db",
        MaximumPoolSize = 10
    };
    var factory = new ConnectionFactory(new SqliteAdapter());
    using var manager = new ConnectionManager(factory, configuration);
    using var connection = manager.Open(configuration);
    // connection verwenden
    manager.Return(connection);

ConnectionConfiguration validiert Provider, ConnectionString sowie Pool-
Grenzen. ConnectionFactory.Create() delegiert an den Adapter. ConnectionPool
bietet Rent(), Return(), Clear() und Dispose(). IConnectionManager definiert
Open(), Return() und Dispose(); ConnectionManager verbindet alle Bausteine.

===== 6. Crypto und Key Management =====

    using RSEV.Utilities.Crypto;

    var provider = new HamacProvider(KeyManager.Generate(32));
    byte[] signature = provider.Compute(Encoding.UTF8.GetBytes("payload"));
    bool valid = provider.Verify(Encoding.UTF8.GetBytes("payload"), signature);
    var digest = new Sha256Provider().Compute(Encoding.UTF8.GetBytes("payload"));

HamacProvider verwendet HMACSHA256. Sha256Provider verwendet SHA256. Beide
implementieren Algorithm, Compute() und Verify() mit konstantzeitnahem
Vergleich. KeyManager erzeugt Schlüssel mit RandomNumberGenerator. Schlüssel
niemals loggen oder in Konfigurationsdateien im Repository speichern.

===== 7. API Tools und Policies =====

    using RSEV.Utilities.Api;

    var request = new ApiRequest {
        Method = HttpMethod.Get,
        Uri = new Uri("https://example.test/health"),
        Headers = new Dictionary<string, string> { ["Accept"] = "application/json" }
    };
    var client = new HttpApiClient(new HttpClient());
    ApiResponse response = await client.SendAsync(request);
    if (!response.IsSuccess) Console.Error.WriteLine(response.StatusCode);

IApiClient definiert SendAsync(). HttpApiClient verwendet HttpClient,
RetryPolicy und RateLimitPolicy. RetryPolicy führt fehlgeschlagene 5xx-
Requests bis MaxAttempts mit Delay erneut aus. RateLimitPolicy kontrolliert
RequestsPerWindow und Window.

===== 8. Runtime API Builder und RuntimeClient =====

    public sealed class HealthEndpoint : BaseRuntimeEndpoint
    {
        public HealthEndpoint() : base("/health") { }
        protected override Task<EndpointResponse> OnHandleAsync(
            EndpointRequest request, CancellationToken token)
            => Task.FromResult(EndpointResponse.FromText(200, "ok"));
    }

    var host = new RuntimeApiHost();
    host.Register(new HealthEndpoint());
    await host.StartAsync(8080);

RuntimeApiHost akzeptiert Endpoints über RuntimeEndpointController. Ein
Endpoint implementiert IRuntimeEndpoint bzw. erbt BaseRuntimeEndpoint.
EndpointRequest, EndpointResponse und der manuelle HTTP/1.1-Parser bilden die
Host-Seite.

    var request = new RuntimeRequestBuilder()
        .WithRoute("/health")
        .WithPayload(new { ping = true })
        .WithHeader("X-Request-Id", "demo")
        .Build();
    var runtimeClient = new RuntimeApiClient("127.0.0.1", 8080);
    var runtimeResponse = await runtimeClient.SendAsync(request);

RuntimeRequestBuilder bietet WithRoute, WithPayload, WithHeader, BindSession,
Build und BuildBytes. RuntimeApiClient bietet CallAsync, SendAsync und
automatische Session-Erneuerung. TcpTransport, TcpConnection, TlsSettings und
Networking.Security kapseln TCP/TLS.

===== 9. Access Token und Session =====

    using RSEV.Utilities.Session;
    using System.Text;

    var tokens = new JwtTokenProvider(Encoding.UTF8.GetBytes("signing-secret"));
    var sessions = new SessionManager(tokens);
    SessionContext session = sessions.Create("user-123");
    if (sessions.IsValid(session)) Console.WriteLine(session.AccessToken!.Value);

AccessToken und RefreshToken enthalten Value, ExpiresAt und IsExpired.
JwtTokenProvider erzeugt und validiert HS256-JWTs mit System.Text.Json und
System.Security.Cryptography. SessionContext verbindet Subject und Tokens.
ISessionManager definiert Create() und IsValid().

===== 10. Events, Messaging, Commands und Controller =====

    var registry = new EventRegistry<string>();
    registry.SubscribeHandler((sender, value) => Console.WriteLine(value));
    registry.Publish("demo", "order.created");

IEvent, IEventHandler, IEventListener, IEventRegistry und EventRegistry
bilden typisierte Events. IEventBus und BaseEventBus bündeln mehrere
Registries. IMessageBus, BaseMessageBus und MessageBusRegistry bilden das
Message-System. ICommand, BaseCommand und CommandHandler bilden Commands;
IController und BaseController bilden Controller.

===== 11. Runtime-Prozesse =====

    var processes = new RuntimeProcessController(logger);
    var worker = new PythonProcess("worker", "worker.py");
    processes.Register(worker);
    await processes.StartAsync("worker");
    var health = await processes.CheckAllHealthAsync();
    await processes.StopAsync("worker");

RuntimeProcessController bietet Register, RegisterRange, Exists, Get, GetAll,
FindByName, FindByState, Unregister, StartAsync, StopAsync, StartByNameAsync,
StopByNameAsync, StartAllAsync, StopAllAsync, CheckAllHealthAsync und
ListProcesses. IRuntimeSubprocess definiert den Prozessvertrag.
DotNetProcess, NodeProcess, PythonProcess, ShellProcess und ExecutableProcess
sind konkrete Adapter. ExecutableResolver löst ausführbare Dateien.

===== 12. Plugins und Discovery =====

    public sealed class MetricsPlugin : IGeneratorPlugin
    {
        public string Name => "metrics";
        public Version Version => new(1, 0, 0);
        public void Generate(GeneratorContext context) { }
    }

    var registry = new GeneratorPluginRegistry();
    registry.Register(new MetricsPlugin());
    registry.Generate(new GeneratorContext());

IGeneratorPlugin definiert Name, Version und Generate(context). Generator-
PluginRegistry bietet Register, All und Generate. GeneratorPluginLoader findet
DLLs über Assembly.LoadFrom und instanziiert konkrete Plugin-Typen.
IPlugin/PluginRegistry bilden zusätzlich den Runtime-Lifecycle mit OnEnable,
OnDisable, OnReload und asynchronen Varianten.

===== 13. Fehler, TLS und Sicherheit =====

  * DbProviderFactories erhält nur explizit registrierte Provider.
  * HTTP-Requests mit CancellationToken abbrechen.
  * TLS-Optionen nicht mit AllowInvalidCertificates in Produktion betreiben.
  * Tokens und Schlüssel nie loggen.
  * Plugins und DLL-Verzeichnisse nur aus vertrauenswürdigen Pfaden laden.
  * IDisposable-Verbindungen und Streams deterministisch schließen.
