====== RSEV Utilities PHP Wiki ======

Willkommen im PHP-Wiki von RSEV Utilities 2.0. Diese Dokumentation beschreibt
die vollständige öffentliche Framework-Oberfläche der PHP-8.5-Implementierung.
Die Beispiele verwenden PHP-Syntax und den eingebauten PSR-4-kompatiblen
Autoloader aus `RSEV/Init.php`.

  * API-Referenz.txt — Klassen, Interfaces und öffentliche Methoden
  * Die Themen Architektur, Datenbank, API, Crypto, Session, Runtime, Events und
  Plugins sind in den folgenden Abschnitten dieser Startseite strukturiert.

===== 1. Installation und Bootstrap =====

Das Framework benötigt PHP >= 8.5. Es wird keine externe Runtime-Bibliothek
benötigt.

<code php>
<?php
require __DIR__ . '/php/php_8.5/RSEV/Init.php';

$application = new RSEV\Core\Application();
$application->run(function (RSEV\Core\Application $app): void {
    $app->apiHost()->start();
});
</code>

`Init.php` definiert `RSEV_EXEC` und registriert den Autoloader. Dateien unter
`RSEV/` sollten über diesen Bootstrap geladen werden.

===== 2. Architektur =====

<code text>
Application
├── Container / Config
├── RuntimeApiHost / Router / HTTP-Modelle
├── RuntimeProcessController
├── EventBus / EventRegistry
├── DatabaseAdapter → ConnectionFactory → ConnectionPool
├── CryptoProvider → JwtTokenProvider → SessionManager
└── PluginLoader / PluginRegistry
</code>

Das Framework verwendet eingebaute PHP-Klassen und Erweiterungen: `PDO`,
Streams, `hash`, `hash_hmac`, `random_bytes`, `DateTimeImmutable` und JSON.
Konkrete PDO-Treiber werden von der Anwendung bereitgestellt und sind keine
Framework-Abhängigkeit.

===== 3. Konfiguration, DI, Collections und Utilities =====

===== Globale Konfiguration =====

<code php>
RSEV\Config\Config::set('app.name', 'demo');
$name = RSEV\Config\Config::get('app.name', 'unknown');

$loader = new RSEV\Config\ConfigLoader();
$loader->load(__DIR__ . '/config.php');
$all = $loader->all();
</code>

`Config` bietet statisches `set()`/`get()`. `ConfigLoader` lädt eine PHP-
Konfigurationsdatei und stellt `load()`, `get()` und `all()` bereit.

===== Dependency Injection =====

<code php>
$container = new RSEV\Container\Container();
$container->bind('clock', static fn (): DateTimeImmutable => new DateTimeImmutable());
$container->singleton('config', static fn (): array => ['env' => 'dev']);

$clock = $container->get('clock');
</code>

`bind()` erzeugt bei jedem `get()` einen Wert. `singleton()` cached den ersten
Wert. Resolver erhalten den Container als Argument.

===== Collections und Hilfsfunktionen =====

<code php>
$list = new RSEV\Collection\ArrayList();
$list->add('a');
$upper = $list->map('strtoupper')->toArray();
$filtered = $list->filter(static fn (string $value): bool => $value !== 'a');

$map = new RSEV\Collection\HashMap();
$map->put('answer', 42);
$answer = $map->get('answer');
</code>

`ArrayList` unterstützt `add`, `get`, `remove`, `map`, `filter`, `toArray`,
`getIterator` und `count`. `HashMap unterstützt `put`, `get`, `containsKey`,
`remove`, `keys`, `values`, `getIterator` und `count`. Die Utility-Klassen
`Arr`, `Str`, `MathUtil`, `Optional` und `Stream` kapseln Array-, String-,
Mathematik-, Optional- und Iterationsoperationen.

===== 4. Logging und Kommunikation =====

<code php>
$logger = new RSEV\Logging\Logger(__DIR__ . '/logs');
$logger->info('Application started');
$logger->error('Request failed');
$logger->log('custom entry', 'audit.log');

$audit = new RSEV\Logging\AuditLogger(__DIR__ . '/logs');
$audit->audit('User signed in');
</code>

`Logger` schreibt `info()`, `error()` und allgemeine `log()`-Einträge. Die
Datei wird relativ zum konfigurierten Verzeichnis geöffnet. `AuditLogger`
ergänzt `audit()`. `DatabaseLogger` schreibt über `DBController` in eine
Datenbank.

===== SMTP und Mail =====

`MailOptions`, `SystemMail`, `BaseSmtpClient`, `RawSmtpClient`,
`AsyncSmtpClient`, `RetrySmtpClient` und die SMTP-Helfer unterstützen native
SMTP-Kommunikation, Authentifizierung, STARTTLS und Wiederholungen. Es wird
keine Mail-Bibliothek benötigt.

<code php>
$options = new RSEV\Communication\MailOptions();
$options->smtpHost = 'smtp.example.test';
$options->mailFrom = 'from@example.test';
$options->mailTo = 'to@example.test';
$options->subject = 'RSEV';
$options->mailBody = 'Hello';

$mail = new RSEV\Communication\SystemMail($options);
$mail->send();
</code>

===== 5. Database und Connection Manager =====

Die Adapter `MySqlAdapter`, `SqlServerAdapter`, `PostgreSqlAdapter` und
`SqliteAdapter` implementieren `IDatabaseAdapter`. Sie erzeugen PDO-Objekte
aus einer `ConnectionConfiguration`.

<code php>
$configuration = new RSEV\Connection\ConnectionConfiguration(
    provider: 'sqlite',
    connectionString: 'sqlite:' . __DIR__ . '/app.db',
);

$adapter = new RSEV\Database\SqliteAdapter();
$factory = new RSEV\Connection\ConnectionFactory($adapter);
$manager = new RSEV\Connection\ConnectionManager($factory, $configuration);

$pdo = $manager->open($configuration);
$pdo->exec('CREATE TABLE IF NOT EXISTS health (value TEXT)');
$manager->giveBack($pdo);
</code>

`ConnectionConfiguration` validiert Provider, DSN sowie Pool-Grenzen.
`ConnectionFactory::create()` delegiert an den Adapter. `ConnectionPool`
stellt `rent()`, `giveBack()` und `clear()` bereit. `ConnectionManager`
implementiert `IConnectionManager` mit `open()` und `giveBack()`.

Für SQL-Ausführung existiert zusätzlich `Communication\DBController`:

<code php>
$db = new RSEV\Communication\DBController([
    'dsn' => 'sqlite:' . __DIR__ . '/app.db',
]);
$rows = $db->query('SELECT value FROM health WHERE value = :value', [':value' => 'ok']);
$db->execute('INSERT INTO health(value) VALUES (:value)', [':value' => 'ok']);
</code>

Anwendungscode ist für konkrete PDO-Treiber verantwortlich. Zugangsdaten
gehören nicht in Quellcode oder Logs.

===== 6. Crypto und Session =====

<code php>
$hamac = new RSEV\Crypto\HamacProvider('a-long-secret-key');
$signature = $hamac->compute('payload');
if (!$hamac->verify('payload', $signature)) {
    throw new RuntimeException('Signature mismatch');
}

$sha = new RSEV\Crypto\Sha256Provider();
$digest = $sha->compute('payload');
$key = new RSEV\Crypto\KeyManager()->generate(32);
</code>

`HamacProvider` verwendet `hash_hmac('sha256', ...)`, `Sha256Provider`
verwendet `hash('sha256', ...)`. Beide bieten `algorithm()`, `compute()` und
`verify()`. `KeyManager::generate()` erzeugt zufällige Schlüssel mit
`random_bytes()`.

===== JWT und Session Manager =====

<code php>
$tokens = new RSEV\Session\JwtTokenProvider('jwt-signing-secret');
$sessions = new RSEV\Session\SessionManager($tokens);
$context = $sessions->create('user-123');

if ($sessions->isValid($context)) {
    echo $context->accessToken->value;
}
</code>

`AccessToken` und `RefreshToken` enthalten Wert und Ablaufzeit sowie
`isExpired()`. `JwtTokenProvider` implementiert `create()` und `validate()`.
`SessionContext` verbindet Subject, Access Token und Refresh Token.

===== 7. API, Routing und HTTP =====

===== Runtime API Host =====

<code php>
final class HealthEndpoint extends RSEV\Api\BaseRuntimeEndpoint
{
    protected function onHandle(RSEV\Api\EndpointRequest $request): RSEV\Api\EndpointResponse
    {
        return RSEV\Api\EndpointResponse::fromJson(['status' => 'ok']);
    }
}

$host = new RSEV\Api\RuntimeApiHost();
$host->register(new HealthEndpoint('/health'));
$host->start();
</code>

`IRuntimeEndpoint` definiert `route()` und `handle()`. `BaseRuntimeEndpoint`
implementiert Routing und delegiert an `onHandle()`. `RuntimeEndpointController`
registriert und löst Endpoints auf. `EndpointRequest` bietet Methode, Route,
Header und `payload()`. `EndpointResponse` bietet `fromText()`, `fromJson()`
und `toHttp()`.

===== Runtime Client und API Tools =====

<code php>
$request = (new RSEV\Api\RuntimeRequestBuilder())
    ->withRoute('/health')
    ->withPayload(['ping' => true])
    ->withHeader('X-Request-Id', 'demo')
    ->build();

$client = new RSEV\Api\RuntimeApiClient('127.0.0.1', 8080);
$response = $client->send($request);
if ($response->isError()) echo $response->getBodyAsText();
</code>

`RuntimeRequestBuilder` unterstützt `withRoute`, `withPayload`, `withHeader`,
`bindSession`, `build` und `buildBytes`. `RuntimeResponse` bietet
`isError()`/`getBodyAsText()`. Für allgemeine HTTP-Aufrufe implementieren
`ApiRequest`, `ApiResponse`, `IApiClient`, `HttpApiClient`, `RetryPolicy` und
`RateLimitPolicy` einen separaten Vertrag.

===== 8. Events, Controller und Commands =====

<code php>
$events = new RSEV\Event\EventRegistry('orders');
$events->subscribeListener(static function ($sender, $event): void {
    // listener
});
$events->publish($this, ['id' => 7]);

$bus = new RSEV\Event\EventBus();
$bus->registerEventRegistry($events, 'order.created');
$bus->publish('order.created', ['id' => 7]);
</code>

`EventRegistry` verwaltet Handler/Listener mit `subscribe`,
`subscribeHandler`, `subscribeListener`, `unsubscribe`, `handlers`,
`listeners`, `publish` und `publishAsync`. `EventBus` bietet
`registerEventRegistry`, `getRegistry`, `hasRegistry`, `getAllEventNames` und
`publish`. `EventDispatcher` ist die einfache `listen`/`dispatch`/
`forget`-Variante.

`ControllerInterface`/`BaseController` kapseln `handle()`/`process()`.
`BaseController`, `ICommand`, `BaseCommand`, `CommandHandler` und die Registry
bilden das Command-System. `Router` ordnet Routen ihren Callables zu.

===== 9. Runtime und Prozesse =====

`RuntimeProcessController` verwaltet `IRuntimeSubprocess`-Implementierungen:
`ShellProcess`, `ExecutableProcess`, `PythonProcess` und `DotNetProcess`.
Der gemeinsame Vertrag enthält `processId`, `name`, `executable`,
`arguments`, `state`, `exitCode`, `properties`, `start`, `stop`, `restart`
und `checkHealth`. `ProcessManager` bietet die vereinfachte Kommando-API.

<code php>
$process = new RSEV\Processing\PythonProcess('worker', 'worker.py');
$process->start();
$healthy = $process->checkHealth();
$process->stop();
</code>

`Application::container()`, `processController()`, `apiHost()` und `run()`
verbinden die zentralen Dienste. `FirstRun`, `ExitCodes` und
`UnexpectedExitHandler` unterstützen Lifecycle und kontrolliertes Beenden.

===== 10. Plugins und Laden =====

<code php>
final class MetricsPlugin implements RSEV\Plugin\IGeneratorPlugin
{
    public function name(): string { return 'metrics'; }
    public function version(): string { return '1.0.0'; }
    public function generate(array $context): void { $context['metrics'] = true; }
}

$registry = new RSEV\Plugin\PluginRegistry();
$registry->register(new MetricsPlugin());
$registry->generate([]);
</code>

`PluginInterface` definiert `boot()`. `PluginLoader::load()` lädt Klassen,
`loadGenerator()` lädt versionierte `IGeneratorPlugin`-Instanzen in eine
`PluginRegistry`. Die Registry bietet `register()`, `all()` und `generate()`.

===== 11. Fehlerbehandlung und Sicherheit =====

  * PDO-Fehler werden als Exceptions konfiguriert; SQL immer parametrisieren.
  * JWT- und HMAC-Schlüssel ausschließlich aus sicherer Konfiguration laden.
  * `RuntimeApiHost` nur an vertrauenswürdige Interfaces binden und TLS/Reverse
  Proxy an der Anwendungsschicht konfigurieren.
  * Prozesse und Plugin-Klassen nur aus kontrollierten Verzeichnissen laden.
  * Responses und Logs dürfen keine Passwörter, Tokens oder Connection Strings
  enthalten.

===== 12. Vollständige API-Referenz =====

Die Datei API-Referenz.txt listet jedes RSEV-Namespace-Verzeichnis, jede
öffentliche Klasse bzw. jedes Interface und deren öffentliche Einstiegsmethoden.
Die Quelle bleibt die verbindliche technische Referenz.
