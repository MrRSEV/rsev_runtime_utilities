# RSEV Utilities - PHP

PHP implementation of the RSEV Utilities framework.

## Overview

This package provides a native PHP 8.5 implementation of the RSEV utility architecture.

Current release: **2.0.0**.

It follows the same design principles as the other platform variants while remaining idiomatic to PHP.

The framework is designed as a lightweight, include-based library without external dependencies.

## Features

* Communication layer
* Config system with structured loading
* Controller base
* Logging system
* Plugin system
* Runtime safety
* Process management
* Typed event registries and event bus
* Runtime API host and client
* First-run bootstrap support
* Database adapters for MySQL, SQL Server, PostgreSQL and SQLite
* Provider-neutral connection factory, pool and connection manager
* HAMAC/HMAC-SHA256 and SHA-256 crypto providers
* JWT access tokens, refresh tokens and session management
* API request/response models with retry and rate-limit policies
* Versioned generator plugin contracts and plugin registry

## Installation

Include the PHP module entry point:

```php
require 'path/to/php/php_8.5/RSEV/Init.php';
```

Composer is not required at runtime. The included `composer.json` is optional
metadata for projects that want Composer autoloading; the framework modules
themselves do not require third-party packages. The `Init.php` entry point
remains the dependency-free bootstrap.

## Structure

```text
php/
└── php_8.5/
    └── RSEV/
        ├── Collection/
        ├── Communication/
        ├── Config/
        ├── Container/
        ├── Controller/
        ├── Core/
        ├── Event/
        ├── Api/
        ├── Connection/
        ├── Crypto/
        ├── Database/
        ├── Http/
        ├── Logging/
        ├── Plugin/
        ├── Processing/
        ├── Routing/
        ├── Runtime/
        ├── Session/
        ├── Stream/
        ├── Utils/
        └── Init.php
```

## Configuration

The included first-run helper can generate a PHP config file if it does not exist yet.

Example path:

```text
config/config_inc.php
```

## Logging

Default log directory:

```text
logs/
```

By default the logger writes to `latest.log`. Audit logging can target `audit.log`.

## Plugins

Plugins must implement:

```php
RSEV\Plugin\PluginInterface
```

They are loaded via `PluginLoader`.

Generator extensions implement `RSEV\Plugin\IGeneratorPlugin` and expose a
name, version and `generate()` operation. `PluginRegistry` handles registration
and execution without introducing a package manager dependency.

## Database, Crypto, API and Session

The 2.0 modules follow the common manifest contract:

* `IDatabaseAdapter` with MySQL, SQL Server, PostgreSQL and SQLite adapters
* `ConnectionConfiguration`, `ConnectionFactory`, `ConnectionPool` and
  `ConnectionManager`
* `HamacProvider`, `Sha256Provider` and `KeyManager`
* `IApiClient`, `HttpApiClient`, `ApiRequest`, `ApiResponse`, `RetryPolicy` and
  `RateLimitPolicy`
* `IAccessTokenProvider`, `JwtTokenProvider`, `ISessionManager`,
  `SessionManager`, `AccessToken`, `RefreshToken` and `SessionContext`

Example:

```php
$crypto = new RSEV\Crypto\HamacProvider('a-long-secret-key');
$signature = $crypto->compute('payload');
$valid = $crypto->verify('payload', $signature);

$tokens = new RSEV\Session\JwtTokenProvider('jwt-signing-secret');
$accessToken = $tokens->create('user-42', 900);
```

## Runtime

* Exit handling via `UnexpectedExitHandler`
* Exit codes defined in `ExitCodes`
* Cleanup of running processes through `ProcessManager`
* Managed subprocesses through `RuntimeProcessController`
* API endpoints through `RuntimeApiHost`
* Process adapters for shell, Python, Node.js, .NET, and arbitrary executables

`RuntimeApiHost` is dependency-free and can be serviced from an application loop
with `tick()`. `RuntimeApiClient` uses the same HTTP/1.1-style request format.

## Composer

The package metadata is included in `composer.json` and exposes the `RSEV\\` PSR-4
namespace. The package version is `2.0.0`.

## Status

| Platform | Status |
| -------- | ------ |
| PHP 8.5 | Active |

## Documentation

* [Platform Wiki](../Doku/README.txt)
* [API-Referenz](../Doku/API-Referenz.txt)
* [Repository Changelog](../../../CHANGELOG.md)



