# RSEV Utilities - PHP

PHP implementation of the RSEV Utilities framework.

## Overview

This package provides a native PHP 8.5 implementation of the RSEV utility architecture.

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
* First-run bootstrap support

## Installation

Include the PHP module entry point:

```php
require 'path/to/php/php_8.5/RSEV/Init.php';
```

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

## Status

| Platform | Status |
| -------- | ------ |
| PHP 8.5 | Active |



