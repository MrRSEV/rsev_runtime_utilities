====== RSEV Utilities Python Wiki ======

Dieses Wiki beschreibt die Python-3.10+-Implementierung. Die Beispiele sind
reine Python-Syntax. Die Framework-Module verwenden die Standardbibliothek;
die YAML-Konfiguration besitzt einen eingebauten Parser für den unterstützten
Subset und benötigt keine PyPI-Laufzeitabhängigkeit.

===== Wiki-Dateien =====

  * README.txt       Einstieg, Architektur und Beispiele
  * API-Referenz.txt Module, Klassen und Funktionen

===== 1. Installation und Einstieg =====

    from rsev_utilities import BaseRuntimeContext, SystemLog

    logger = SystemLog()
    context = BaseRuntimeContext(logger=logger)
    context.set('environment', 'dev')
    logger.info('RSEV started')

Die Bibliothek kann direkt aus `python/rsev_utilities` importiert werden. Der
öffentliche Sammel-Import in `rsev_utilities/__init__.py` exportiert die
Runtime-, API-, Database-, Crypto-, Session- und Plugin-Verträge.

===== 2. Architektur =====

    rsev_utilities
    +- core / configuration / logging
    +- api / communication / networking-nahe Runtime
    +- database / connection / crypto / session
    +- events / command / controller / di
    +- plugin / loading
    +- runtime / processing

Python verwendet Protokoll- und ABC-Verträge, Dataclasses für Modelle und
Dependency Injection über Konstruktoren oder `Container`/`GraphContainer`.

===== 3. Core, Konfiguration und Dependency Injection =====

Core-Helfer:

    from rsev_utilities.core import StringUtils, DateUtils, FileUtils, SystemUtils

`StringUtils`, `DateUtils`, `FileUtils` und `SystemUtils` kapseln die
entsprechenden String-, Datums-, Datei- und Betriebssystemoperationen.

Konfiguration:

    from rsev_utilities.configuration import JsonConfig, YamlConfig

    config = JsonConfig()
    config.set('service.port', 8080)
    config.save('app.json')
    config.load('app.json')

    YamlConfig.save('app.yaml', {'service': {'port': 8080}})
    values = YamlConfig.load('app.yaml')

`BaseConfig`, `IConfig`, `JsonConfig`, `YamlConfig`, `XmlConfig`, `ConfConfig`
und `AutoReloadConfig` stellen Load-/Save- und Wertzugriff bereit. YAML
unterstützt JSON als YAML-Subset sowie einfache eingerückte Maps/Listen.

Dependency Injection:

    from rsev_utilities.di import Container, GraphContainer

    container = Container()
    container.register('clock', lambda: datetime.now(timezone.utc))
    clock = container.resolve('clock')

`inject()` injiziert benannte Abhängigkeiten. `GraphContainer` kann einen
Abhängigkeitsgraphen auflösen.

===== 4. Logging =====

    from rsev_utilities.logging import BaseLogger, AdvancedLogger

    logger = BaseLogger.get('application')
    logger.info('service started')
    logger.error('request failed')

    detailed = AdvancedLogger.create('worker', level=logging.DEBUG)
    detailed.debug('debug information')

`BaseLogger.get()` liefert einen Standard-Logger. `AdvancedLogger.create()`
konfiguriert Level, StreamHandler und ein strukturiertes Zeit-/Level-/Name-
Format. Runtime- und Prozessklassen akzeptieren Logger-Anbindungen in ihren
Kontexten.

===== 5. Communication und SMTP =====

    from rsev_utilities.communication.smtp import RawSmtpClient, MailOptions, SystemMail

    options = MailOptions(
        smtp_host='smtp.example.test',
        mail_from='from@example.test',
        mail_to='to@example.test',
        subject='RSEV',
        mail_body='Hello',
    )
    SystemMail(options, RawSmtpClient()).send()

`ISmtpClient`, `BaseSmtpClient`, `RawSmtpClient`, `AsyncSmtpClient` und
`RetrySmtpClient` implementieren SMTP mit Authentifizierung, STARTTLS und
Retry-Verhalten. `communication.system_mail.SystemMail` ist die vereinfachte
Fassade. Zugangsdaten nicht loggen.

===== 6. Database und Connection Manager =====

Die vier provider-neutralen Klassen heißen `MySqlAdapter`, `SqlServerAdapter`,
`PostgreSqlAdapter` und `SqliteAdapter`. Eine DB-API-kompatible Connection-
Factory wird injiziert, deshalb wird kein konkreter Treiber importiert.

    from rsev_utilities.database import ConnectionConfiguration, SqliteAdapter
    from rsev_utilities.connection import ConnectionFactory, ConnectionManager

    configuration = ConnectionConfiguration('sqlite', 'app.db')
    adapter = SqliteAdapter(lambda cfg: open(cfg.connection_string, 'a+b'))
    factory = ConnectionFactory(adapter)
    manager = ConnectionManager(factory, configuration)
    connection = manager.open(configuration)
    manager.give_back(connection)
    manager.close()

`ConnectionConfiguration` validiert Provider, Connection String und Pool-
Grenzen. `IDatabaseAdapter.create_connection()` delegiert an die injizierte
Factory. `ConnectionPool` bietet thread-sichere `rent()`, `give_back()` und
`clear()`-Operationen. `IConnectionManager` und `ConnectionManager` verbinden
Factory und Pool.

===== 7. Crypto und Key Management =====

    from rsev_utilities.crypto import HamacProvider, Sha256Provider, KeyManager

    hamac = HamacProvider(b'secret')
    signature = hamac.compute(b'payload')
    assert hamac.verify(b'payload', signature)

    digest = Sha256Provider().compute(b'payload')
    key = KeyManager().generate(32)

`HamacProvider` nutzt `hmac.new(..., hashlib.sha256)`, `Sha256Provider` nutzt
`hashlib.sha256`. `ICryptoProvider` definiert `compute()` und `verify()`.
`KeyManager` erzeugt zufällige Schlüssel über `secrets.token_bytes()`.

===== 8. API und Policies =====

    from rsev_utilities.api import ApiRequest, HttpApiClient

    request = ApiRequest('GET', 'https://example.test/health', {'Accept': 'application/json'})
    response = HttpApiClient().send(request)
    if not response.is_success:
        print(response.status_code, response.body)

`IApiClient.send()` ist der Vertrag. `HttpApiClient` verwendet `urllib`,
`RetryPolicy` und `RateLimitPolicy`. `ApiRequest` enthält Methode, URL, Header
und Body. `ApiResponse` enthält Status, Header und Body sowie `is_success`.

Die bereits vorhandene Runtime-API nutzt zusätzlich:

    from rsev_utilities.api import RuntimeApiClient, RuntimeRequestBuilder

    client = RuntimeApiClient('127.0.0.1', 8080)
    response = client.call('/health', {'ping': True})

`RuntimeRequestBuilder` bietet `with_route`, `with_payload`, `with_header`,
`bind_session`, `build` und `build_bytes`. `RuntimeApiClient` bietet `call`,
`send` und Sessionbindung. `RuntimeApiHost`, Endpoint-Controller,
`EndpointRequest`, `EndpointResponse` und `BaseRuntimeEndpoint` bilden die
Serverseite.

===== 9. Access Tokens, JWT und Session =====

    from datetime import timedelta
    from rsev_utilities.session import JwtTokenProvider, SessionManager

    tokens = JwtTokenProvider(b'signing-secret')
    sessions = SessionManager(tokens)
    session = sessions.create('user-123')
    assert sessions.is_valid(session)

`AccessToken` und `RefreshToken` sind immutable Modelle mit `value`,
`expires_at` und `is_expired`. `JwtTokenProvider.create()` erzeugt HS256-
Tokens, `validate()` prüft Signatur und Ablauf. `SessionContext` verbindet
Subject und beide Tokens. `ISessionManager` definiert `create()` und
`is_valid()`.

===== 10. Events, Commands und Controller =====

    from rsev_utilities.events import EventRegistry

    registry = EventRegistry('orders')
    registry.subscribe_handler(lambda sender, event: print(event))
    registry.publish('demo', {'id': 7})

`Event`, `EventHandler`, `EventListener`, `EventRegistry`, `EventBus` und die
Base-Klassen implementieren typisierte Event-Hooks mit Subscribe, Unsubscribe,
Publish und Async-Varianten.

`ICommand`, `BaseCommand`, `Command`, `CommandHandler`, `CommandRegistry` und
die CLI-/Discord-/Minecraft-Adapter bilden das Command-System. `IController`
und `BaseController` kapseln Controller-Lifecycle.

===== 11. Runtime, Prozesse und Loading =====

    from rsev_utilities.runtime.processing import PythonProcess, RuntimeProcessController

    processes = RuntimeProcessController()
    worker = PythonProcess('worker', 'worker.py')
    processes.register(worker)
    processes.start('worker')
    worker.check_health()
    processes.stop('worker')

`IRuntimeSubprocess`, `RuntimeSubprocessBase`, `SubprocessState` und
`ProcessLogLevel` bilden den Prozessvertrag. `ShellProcess`, `PythonProcess`,
`NodeProcess`, `DotNetProcess` und `ExecutableProcess` sind konkrete Adapter.
`RuntimeProcessController` registriert, startet, stoppt, listet und prüft
Prozesse. `BaseRuntimeContext`, `ShutdownSummary`, `UnexpectedExitHandler` und
`ExitCode` steuern den Application-Lifecycle.

`AssemblyLoader` und `TypeDiscovery` unterstützen Python-Module bzw.
Datei-/Typ-Discovery, ohne einen externen Loader vorauszusetzen.

===== 12. Plugins und Hot Reload =====

    from rsev_utilities.plugin import BasePlugin, PluginLoader

    class MetricsPlugin(BasePlugin):
        name = 'metrics'
        version = '1.0.0'
        def on_enable(self, context=None):
            super().on_enable(context)

    loader = PluginLoader()
    loader.load_from_directory('./plugins')
    loader.enable_all()

`IPlugin`, `BasePlugin`, `PluginLoader`, `PluginContext`,
`EntryPointPluginLoader`, `LifecycleManager` und `HotReloadManager` bilden den
klassischen Plugin-Lifecycle. `IGeneratorPlugin` und
`GeneratorPluginRegistry` ergänzen Name, Version, Discovery, Registrierung und
`generate(context)` für manifestbezogene Erweiterungen.

===== 13. Fehler- und Sicherheitsregeln =====

  * Secrets, JWTs, SMTP- und Connection-Daten niemals loggen.
  * API-URLs und TLS-Zertifikate validieren; bei HTTPS eigene SSL-Kontexte nutzen.
  * Datenbanktreiber nur über injizierte Factories anbinden.
  * Keine Shell-Argumente aus ungeprüften Benutzereingaben zusammensetzen.
  * Token-Ablauf und Refresh-Token immer serverseitig prüfen.
