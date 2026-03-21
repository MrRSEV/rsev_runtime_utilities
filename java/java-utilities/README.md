# Java-Utilities

A dependency-free, modular utility framework designed for
cross-platform and cross-language system-level development.

## Features
- Native SMTP implementation (no jakarta.mail / javax.mail)
- Optional STARTTLS and SMTP AUTH (LOGIN / PLAIN)
- Unified logging system with optional debug tracing
- Configuration abstractions (JSON, YAML, XML, CONF)
- Plugin-ready architecture
- No external dependencies by design

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
