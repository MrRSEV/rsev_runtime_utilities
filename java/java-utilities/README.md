# Java-Utilities

Current release: **2.0.0**.

A dependency-free, modular utility framework designed for
cross-platform and cross-language system-level development.

## Features
- Native SMTP implementation (no jakarta.mail / javax.mail)
- Optional STARTTLS and SMTP AUTH (LOGIN / PLAIN)
- Unified logging system with optional debug tracing
- Configuration abstractions (JSON, YAML, XML, CONF)
- Plugin-ready architecture
- No external dependencies by design
- Typed event registries and event bus
- Managed subprocesses with lifecycle and health checks
- Dependency-free runtime API host and client
- Database adapter contracts for MySQL, SQL Server, PostgreSQL and SQLite
- Provider-neutral connection factory, pool and connection manager
- HAMAC/HMAC-SHA256 and SHA-256 crypto providers with key management
- JWT access tokens, refresh tokens and session manager contracts
- API request/response models with retry and rate-limit policies
- Versioned generator plugin contracts and plugin registry

## Build and install

```bash
./gradlew build
```

The project remains dependency-free and uses the Java standard library only.
No Maven- or Gradle-runtime dependency is required by the framework.

## Design Goals
- System-level clarity
- No hidden magic
- Cross-language parity (Java / C# / future targets)
- Suitable for servers, tools, and embedded bridges

## Example (SMTP)

```java
BaseLogger.init();

MailOptions opt = new MailOptions();
opt.setSmtpHost("smtp.example.com");
opt.setMailFrom("from@example.com");
opt.setMailTo("to@example.com");
opt.setSubject("Test");
opt.setMailBody("Hello World");

RawSmtpClient client = new RawSmtpClient();
client.setDebugEnabled(true);

SystemMail mail = new SystemMail(opt, client);
mail.send();

```

The 2.0 runtime APIs are available under `de.rsev.utilities.events`,
`de.rsev.utilities.runtime.processes`, and `de.rsev.utilities.api`.

### Runtime examples

Register a managed process and start it through the central controller:

```java
RuntimeProcessController processes = new RuntimeProcessController();
PythonProcess process = new PythonProcess("worker", "worker.py");
processes.register(process);
processes.startAsync(process.getProcessId());
```

For API endpoints, extend `BaseRuntimeEndpoint`, register it with
`RuntimeApiHost`, and call `start()`.

## Database, Crypto, API and Session

The 2.0 modules are available under the following packages:

* `de.rsev.utilities.database` — provider-neutral database adapters
* `de.rsev.utilities.connection` — configuration, factory, pool and manager
* `de.rsev.utilities.crypto` — HAMAC/HMAC-SHA256, SHA-256 and key management
* `de.rsev.utilities.api` — clients, request/response models and policies
* `de.rsev.utilities.session` — JWT, access/refresh tokens and sessions
* `de.rsev.utilities.plugin` — versioned generator plugin contracts

Example:

```java
ICryptoProvider crypto = new HamacProvider("a-long-secret-key".getBytes(StandardCharsets.UTF_8));
byte[] signature = crypto.compute("payload".getBytes(StandardCharsets.UTF_8));
boolean valid = crypto.verify("payload".getBytes(StandardCharsets.UTF_8), signature);
```

## Documentation

* [Platform Wiki](Doku/README.txt)
* [API-Referenz](Doku/API-Referenz.txt)
* [Repository Changelog](../../CHANGELOG.md)
