====== RSEV Utilities Rust Wiki ======

Dieses Wiki beschreibt die Rust-Implementierung von RSEV Utilities. Sie
verwendet ausschließlich `std` und besitzt keine Cargo-Third-Party-
Abhängigkeit. Rust ersetzt Interfaces durch Traits und mutable Vererbung durch
Komposition mit `Arc`, `Mutex` und `RwLock`.

===== Wiki-Dateien =====

  * README.txt       Einstieg, Architektur und Beispiele
  * API-Referenz.txt Module, Traits, Structs und öffentliche Funktionen

===== 1. Build und Einstieg =====

    cargo check
    cargo test

`cargo test` benötigt auf Windows einen konfigurierten nativen Linker. Die
Bibliothek exportiert zentrale Typen in `src/lib.rs`:

    use rsev_utilities::{BaseRuntimeContext, RuntimeContext, SystemLog};

    let context = BaseRuntimeContext::default();
    context.set("environment".into(), "dev".into());

===== 2. Architektur =====

    RuntimeContext
    +- Configuration / Logging / Loading
    +- API Builder / Runtime Client / Networking
    +- DatabaseAdapter -> ConnectionFactory -> ConnectionPool
    +- CryptoProvider -> JwtTokenProvider -> SessionManager
    +- Events / MessageBus / Commands / Plugins
    +- Processes

Alle Synchronisationstypen stammen aus `std::sync`. Anwendungen injizieren
konkrete Datenbank- oder TLS-Backends über Traits; die Kernbibliothek verlinkt
keinen externen Treiber.

===== 3. Configuration und Runtime Context =====

`Config`, `BaseConfig`, `JsonConfig`, `YamlConfig` und `ConfConfig` stellen
String-Key-Value-Konfiguration bereit. `get_value<T>()` nutzt `FromStr`.

    use rsev_utilities::configuration::{BaseConfig, Config};

    let mut config = BaseConfig::default();
    config.set("port".into(), "8080".into());
    let port: Option<u16> = config.get_value("port");

`RuntimeContext` definiert Config, Logger, API-Host, Plugin-Registry,
Message-Bus, Process-Controller, State und Running-Flag. `BaseRuntimeContext`
implementiert diesen Vertrag. `ShutdownSummary`, `OnStart`, `OnStop` und
`FirstRun` unterstützen Lifecycle und kontrolliertes Beenden.

===== 4. Logging =====

    use std::sync::Arc;
    use rsev_utilities::logging::{Logger, SystemLog, LogLevel};

    let logger = Arc::new(SystemLog::default());
    logger.log(LogLevel::Info, "service started");
    logger.log(LogLevel::Error, "request failed");

`Logger` definiert `log()`, `debug()`, `info()`, `warn()`, `error()` und
`critical()`. `SystemLog::new()`/`init()` konfigurieren Verzeichnis und
Datei; `close()` beendet den Writer. LogLevel umfasst Debug, Info, Warn,
Error und Critical.

===== 5. Database und Connection =====

Rust besitzt in `std` keinen JDBC-/PDO-/DB-API-Treiber. Daher modelliert das
Framework die provider-neutralen Verträge und lässt die eigentliche
Verbindung an der Anwendungsschicht erzeugen.

    use std::sync::Arc;
    use rsev_utilities::database::{ConnectionConfiguration, MySqlAdapter};
    use rsev_utilities::connection::{ConnectionFactory, ConnectionManager, IConnectionManager};

    let config = ConnectionConfiguration::new("sqlite", "app.db").unwrap();
    let factory = ConnectionFactory::new(Arc::new(MySqlAdapter));
    let manager = ConnectionManager::new(factory, &config).unwrap();
    let connection = manager.open(&config).unwrap();
    manager.give_back(connection);

`ConnectionConfiguration::new()` und `validate()` prüfen Provider, Connection
String und Pool-Grenzen. `DatabaseAdapter`/`IDatabaseAdapter` definieren
provider() und create_connection(). MySqlAdapter, SqlServerAdapter,
PostgreSqlAdapter und SqliteAdapter erzeugen den standardisierten
`DatabaseConnection`-Handle. `ConnectionFactory::create()`,
`ConnectionPool::rent()`/`give_back()`/`clear()` und
`ConnectionManager::open()`/`give_back()` bilden die Pool-Schicht.

===== 6. Crypto und Key Management =====

    use rsev_utilities::crypto::{CryptoProvider, HamacProvider, Sha256Provider};

    let hamac = HamacProvider::new(b"secret").unwrap();
    let signature = hamac.compute(b"payload");
    assert!(hamac.verify(b"payload", &signature));
    let digest = Sha256Provider.compute(b"payload");

`HamacProvider` implementiert HMAC-SHA256, `Sha256Provider` SHA-256. Die
SHA-256-Kompression und HMAC-Konstruktion liegen vollständig in `crypto.rs`,
weil keine externe Kryptobibliothek verwendet wird. `CryptoProvider` und
`ICryptoProvider` definieren algorithm(), compute() und verify().
`KeyManager::generate()` bereitet Schlüsselmaterial std-only vor; für
produktive Schlüssel soll eine plattformspezifische sichere Entropy-Quelle an
der Anwendungsschicht injiziert werden.

===== 7. API und Networking =====

    use rsev_utilities::api_tools::{ApiRequest, HttpApiClient, IApiClient};

    let client = HttpApiClient::new("127.0.0.1:8080");
    let request = ApiRequest { method: "GET".into(), route: "/health".into(), ..Default::default() };
    let response = client.send(&request).unwrap();
    assert!(response.is_success());

`IApiClient`, `ApiRequest`, `ApiResponse`, `HttpApiClient`, `RetryPolicy` und
`RateLimitPolicy` bilden die API-Tools. HttpApiClient delegiert an den
vorhandenen `RuntimeApiClient`, wiederholt 5xx-Antworten und prüft das
Rate-Limit.

Der Runtime-Host:

    use std::sync::Arc;
    use rsev_utilities::api_builder::{BaseRuntimeEndpoint, EndpointResponse, RuntimeApiHost};

    let host = RuntimeApiHost::new(None);
    host.register(Arc::new(BaseRuntimeEndpoint::new("/health")));
    host.start(8080).unwrap();

`EndpointRequest`, `EndpointResponse`, `RuntimeEndpoint`,
`BaseRuntimeEndpoint`, `RuntimeEndpointController` und `RuntimeApiHost`
bilden Routing und HTTP-Parsing. `runtime_client` enthält
`RuntimeRequestBuilder`, `RuntimeRequest`, `RuntimeResponse` und
`RuntimeApiClient`. `networking` stellt TcpTransport, TcpConnection und
TlsOptions bereit. TLS ist im std-only Kern bewusst ein Boundary-Hook und
meldet für aktivierte TLS-Optionen einen Unsupported-Fehler.

===== 8. Access Tokens und Session =====

    use std::time::Duration;
    use rsev_utilities::session::{IAccessTokenProvider, ISessionManager, JwtTokenProvider, SessionManager};

    let tokens = JwtTokenProvider::new(b"signing-secret").unwrap();
    let sessions = SessionManager::new(tokens);
    let session = sessions.create("user-123").unwrap();
    assert!(sessions.is_valid(&session));

`AccessToken` und `RefreshToken` enthalten Wert und `SystemTime`; beide bieten
`is_expired()`. `JwtTokenProvider` erzeugt und validiert HS256-JWTs. Die
Base64url- und HMAC-Helfer sind std-only. `SessionContext`,
`IAccessTokenProvider`, `ISessionManager` und `SessionManager` bilden die
Session-API.

===== 9. Events, Messaging und Commands =====

    use std::sync::Arc;
    use rsev_utilities::events::{EventHandler, EventRegistry};

    struct Handler;
    impl EventHandler<String> for Handler {
        fn handle(&self, _sender: &str, event: &String) { println!("{event}"); }
    }
    let registry = EventRegistry::new("orders");
    registry.subscribe_handler(Arc::new(Handler));
    registry.publish("demo", &"order.created".to_string());

`EventHandler`, `EventListener`, `EventRegistry`, `EventBus` und
`BaseEventBus` unterstützen Subscribe, Unsubscribe, Count und Publish.
`MessageBus`, `BaseMessageBus` und `MessageBusRegistry` bilden den
Messaging-Vertrag. `Command`, `CommandHandler` und `Controller` kapseln
Command-/Controller-Operationen.

===== 10. Runtime-Prozesse =====

    use rsev_utilities::processes::{PythonProcess, RuntimeProcessController};

    let processes = RuntimeProcessController::default();
    let worker = PythonProcess::new("worker", "worker.py", vec![]);
    processes.register(worker);
    processes.start("worker").unwrap();
    processes.stop("worker").unwrap();

`RuntimeSubprocess`, `CommandProcess`, `ShellProcess`, `PythonProcess`,
`NodeProcess`, `DotNetProcess`, `ExecutableResolver` und
`RuntimeProcessController` kapseln Start, Stop, Restart, State, Exit-Code,
Health und Discovery. `SubprocessState` beschreibt Created, Running,
Stopped, Failed und verwandte Zustände; `ProcessLogLevel` ordnet Prozess-
Ausgaben dem Logger zu.

===== 11. Plugins und Discovery =====

    use std::collections::BTreeMap;
    use std::sync::Arc;
    use rsev_utilities::plugins::{GeneratorPluginRegistry, IGeneratorPlugin};

    struct Metrics;
    impl IGeneratorPlugin for Metrics {
        fn name(&self) -> &str { "metrics" }
        fn version(&self) -> &str { "1.0.0" }
        fn generate(&self, context: &mut BTreeMap<String, String>) { context.insert("enabled".into(), "true".into()); }
    }
    let registry = GeneratorPluginRegistry::default();
    registry.register(Arc::new(Metrics)).unwrap();

`Plugin` und `PluginRegistry` bilden den normalen Runtime-Lifecycle mit
enable()/disable(). `IGeneratorPlugin` ergänzt name(), version() und
generate(context). `GeneratorPluginRegistry` bietet register(), all() und
generate(). Dynamische DLL-Discovery ist wegen der std-only-Vorgabe nicht
Bestandteil der Rust-Kernbibliothek; Plugins werden explizit registriert.

===== 12. Sicherheit und Fehlerbehandlung =====

  * Alle I/O-Funktionen geben `io::Result` oder `Result<T, String>` zurück.
  * Datenbanktreiber, TLS-Backends und echte Entropy-Quellen an der
  Anwendungsschicht injizieren.
  * Keine Secrets, Tokens oder Connection Strings in Logger-/Process-Ausgaben.
  * Prozesse nur mit validierten Argumenten starten.
  * `Arc`/`Mutex`-Guards nach Möglichkeit kurz halten und bei Fehlern sauber
  schließen.
