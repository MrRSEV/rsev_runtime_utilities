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

## Installation

Include the PHP module entry point:

```php
require 'path/to/php/php_8.5/RSEV/Init.php';
```

For Composer-based projects, use the included `composer.json` and load
`vendor/autoload.php`. The legacy `Init.php` entry point remains supported.

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
        ├── Http/
        ├── Logging/
        ├── Plugin/
        ├── Processing/
        ├── Routing/
        ├── Runtime/
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



