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

## Build and install

```bash
./gradlew build
```

The project remains dependency-free and uses the Java standard library only.

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
