====== RSEV Utilities Java Wiki ======

Dieses Wiki beschreibt die Java-2.0-Implementierung vollständig auf Basis der
Java-Standardbibliothek. Das Projekt hat keine Maven-/Gradle-Laufzeit-
Abhängigkeit. JDBC-Treiber und SMTP-Server werden von der Anwendung an der
Systemgrenze bereitgestellt.

===== Wiki-Navigation =====

  * API-Referenz.txt              Klassen, Interfaces und Methoden
  * Die Themen Architektur, Database, Connection, API, Crypto, Session, Runtime,
  Events und Plugins sind in den folgenden Abschnitten dieser Startseite
  strukturiert.

===== 1. Installation und Start =====

Build:

    .\gradlew.bat build

Direkter Standardcompiler:

    javac -encoding UTF-8 -d out $(find de -name '*.java')

Minimaler Einstieg:

    import de.rsev.utilities.logging.BaseLogger;
    import de.rsev.utilities.api.RuntimeApiHost;

    public final class Main {
        public static void main(String[] args) {
            BaseLogger.init();
            RuntimeApiHost host = new RuntimeApiHost();
            host.start();
        }
    }

===== 2. Paketarchitektur =====

    de.rsev.utilities
    +- api                  Runtime-Host, Client und API-Tools
    +- connection/database  JDBC-Verträge, Factory und Pool
    +- crypto/session       SHA/HAMAC, JWT und Session-Lifecycle
    +- config               Konfigurationsabstraktionen
    +- logging              Logger und Log-Rotation
    +- events               Event-Registry und Event-Bus
    +- runtime              Kontext und Prozessverwaltung
    +- communication        SMTP und Mail
    +- command/controller   Commands und Controller
    +- plugin               Plugins, Registry und ServiceLoader

Die vollständige öffentliche Oberfläche steht in API-Referenz.txt.

===== 3. Konfiguration und Runtime-Kontext =====

IConfig, BaseConfig, JsonConfig, YamlConfig, XmlConfig und ConfConfig bilden
die Konfigurationsfamilie. BaseRuntimeContext stellt Basisverzeichnis,
Debug-Flag, RuntimeProcessController und RuntimeApiHost bereit.

    BaseRuntimeContext context = new BaseRuntimeContext(".") {
        // projektspezifischer Kontext kann hier erweitert werden
    };
    String root = context.getBaseDirectory();
    boolean debug = context.isDebug();

Die Konfigurationsklassen besitzen load(), save() und typisierte get()-
Operationen entsprechend ihres Formats. JsonConfig ist derzeit eine
Standardbibliotheks-Abstraktion; eine JSON-Parser-Implementierung kann an der
Anwendungsgrenze ergänzt werden.

===== 4. Logging =====

    BaseLogger.init();
    BaseLogger logger = new BaseLogger();
    logger.debug("Debug information");
    logger.info("Service started");
    logger.warn("Slow operation");
    logger.error("Request failed");
    BaseLogger.close();

BaseLogger initialisiert OS-abhängige Log-Verzeichnisse, schreibt latest.log,
rotates vorhandene Dateien und unterstützt LogLevel INFO, WARNING, ERROR,
CRITICAL und DEBUG sowie LogColor. ILogger und LoggerAdapter erlauben
Einbettung in eigene Logging-Systeme. Für produktive Anwendungen sollte
critical() vor dem Einsatz auf eine konkrete Fehlerstrategie gebunden werden.

===== 5. Kommunikation und SMTP =====

MailOptions enthält SMTP-Host, Absender, Empfänger, Subject und Body.
SystemMail verbindet MailOptions mit ISmtpClient. RawSmtpClient implementiert
das Protokoll ohne jakarta.mail. BaseSmtpClient, SmtpConnection,
SmtpProtocol, SmtpResponse, SmtpAuthHandler und StartTlsHandler bilden die
einzelnen Protokollschritte.

    MailOptions options = new MailOptions();
    options.setSmtpHost("smtp.example.test");
    options.setMailFrom("from@example.test");
    options.setMailTo("to@example.test");
    options.setSubject("RSEV");
    options.setMailBody("Hello");
    new SystemMail(options, new RawSmtpClient()).send();

===== 6. Database und Connection Manager =====

IDatabaseAdapter, MySqlAdapter, SqlServerAdapter, PostgreSqlAdapter und
SqliteAdapter verwenden ausschließlich java.sql.Connection und
DriverManager. Der konkrete JDBC-Treiber ist keine Framework-Abhängigkeit.

    ConnectionConfiguration configuration = new ConnectionConfiguration(
        "sqlite", "jdbc:sqlite:app.db", "", "");
    IDatabaseAdapter adapter = new SqliteAdapter();
    ConnectionFactory factory = new ConnectionFactory(adapter);
    try (ConnectionManager manager = new ConnectionManager(factory, configuration)) {
        Connection connection = manager.open(configuration);
        // PreparedStatement verwenden; niemals SQL mit Benutzereingaben konkatenieren.
        manager.giveBack(connection);
    }

ConnectionConfiguration validiert Provider, URL und Pool-Grenzen. Factory
delegiert create(). ConnectionPool bietet rent(), giveBack(), clear() und
close(). IConnectionManager definiert open(), giveBack() und close();
ConnectionManager verbindet Factory und Pool.

===== 7. Crypto und Key Management =====

    ICryptoProvider hamac = new HamacProvider("secret".getBytes());
    byte[] signature = hamac.compute("payload".getBytes());
    boolean valid = hamac.verify("payload".getBytes(), signature);

    ICryptoProvider sha = new Sha256Provider();
    byte[] digest = sha.compute("payload".getBytes());
    byte[] key = new KeyManager().generate(32);

HamacProvider nutzt HmacSHA256, Sha256Provider SHA-256 und beide verwenden
konstantzeitnahe Vergleiche. Schlüssel niemals loggen oder in Git speichern.

===== 8. Access Tokens und Sessions =====

    IAccessTokenProvider tokens = new JwtTokenProvider("signing-secret".getBytes());
    ISessionManager sessions = new SessionManager(tokens);
    SessionContext context = sessions.create("user-123");
    if (sessions.isValid(context)) {
        System.out.println(context.accessToken().value());
    }

AccessToken und RefreshToken enthalten value(), expiresAt() und isExpired().
JwtTokenProvider implementiert HS256-Erzeugung und Validierung.
SessionManager erzeugt Access- und Refresh-Token und prüft beide Ablaufzeiten.

===== 9. API und HTTP =====

Runtime-Endpunkt:

    RuntimeApiHost host = new RuntimeApiHost();
    host.register(new BaseRuntimeEndpoint("/health") {
        @Override public EndpointResponse onHandle(EndpointRequest request) {
            return EndpointResponse.fromText(200, "ok");
        }
    });
    host.start();

IRuntimeEndpoint definiert route() und handle(). RuntimeEndpointController
registriert und löst Routen auf. EndpointRequest und EndpointResponse bilden
HTTP-Daten.

Allgemeiner API-Client:

    ApiRequest request = new ApiRequest(
        "GET", java.net.URI.create("https://example.test/health"),
        java.util.Map.of("Accept", "text/plain"), new byte[0]);
    IApiClient client = new HttpApiClient(java.net.http.HttpClient.newHttpClient());
    ApiResponse response = client.sendAsync(request).join();

ApiRequest, ApiResponse, HttpApiClient, RetryPolicy und RateLimitPolicy sind
vom Runtime-Host getrennt. RuntimeRequestBuilder unterstützt withRoute(),
withPayload(), withHeader(), bindSession() und build(); RuntimeApiClient
unterstützt setSession(), callAsync() und sendAsync().

===== 10. Events, Commands und Controller =====

    EventRegistry<String> registry = new EventRegistry<>("orders");
    registry.subscribeHandler((sender, event) -> System.out.println(event));
    registry.publish("demo", "order.created");

EventRegistry verwaltet Handler und Listener; EventBus verbindet mehrere
Registries. IEvent, IEventHandler, IEventListener, IEventRegistry und
IEventBus sind die Erweiterungspunkte. ICommand, BaseCommand,
CommandHandler und PluginReload bilden Commands. IController und
BaseController bilden Controller.

===== 11. Prozesse und Runtime =====

RuntimeProcessController bietet register(), registerRange(), exists(), get(),
getAll(), findByName(), findByState(), unregister(), startAsync(),
stopAsync(), startByNameAsync(), stopByNameAsync(), startAllAsync(),
stopAllAsync() und checkAllHealthAsync().

    RuntimeProcessController processes = new RuntimeProcessController();
    PythonProcess worker = new PythonProcess("worker", "worker.py");
    processes.register(worker);
    processes.startAsync("worker");
    processes.checkAllHealthAsync();

IRuntimeSubprocess definiert Prozess-ID, Name, Executable, Argumente, State,
Exit-Code, Properties, startAsync(), stopAsync(), restartAsync() und
checkHealthAsync(). ShellProcess, PythonProcess, NodeProcess, DotNetProcess
und ExecutableProcess sind konkrete Adapter. ExecutableResolver löst
Umgebungsvariablen und Fallbacks.

===== 12. Plugins und Discovery =====

    public final class MetricsPlugin implements IGeneratorPlugin {
        public String name() { return "metrics"; }
        public String version() { return "1.0.0"; }
        public void generate(Map<String, Object> context) { context.put("metrics", true); }
    }

PluginRegistry registriert Plugins deterministisch nach Name. GeneratorPluginLoader
nutzt den Java-ServiceLoader und ruft generate() auf. Das klassische IPlugin /
BasePlugin-Interface bietet zusätzlich onLoad(), onReload(), onUnload(),
isEnabled() und setEnabled().

===== 13. Fehler- und Sicherheitsregeln =====

  * JDBC immer mit PreparedStatement verwenden.
  * Secrets, JWTs und SMTP-Passwörter nicht loggen.
  * RuntimeApiHost nur kontrolliert exponieren und TLS an der Netzgrenze setzen.
  * Prozesse nur mit vertrauenswürdigen Executables starten.
  * Ressourcen mit try-with-resources schließen.
