# RSEV Utilities

A unified utility framework for object-oriented platforms.

Current release: **3.0.0**.

## Overview

RSEV Utilities provides a structured, modular toolbox for system-level development
across multiple OOP ecosystems.

Each platform implementation is:

* Native to its environment
* Architecturally consistent
* Designed with the same philosophy and structure

## Platforms

* Python -> `./python/rsev_utilities`
* Java -> `./java/java-utilities`
* .NET -> `./dotnet/net10/RSEV.Utilities`
* PHP 8.5 -> `./php/php_8.5/RSEV`
* Rust -> `./rust/rsev-utilities`

Each platform has its own implementation, documentation, and release cycle.

## Platform Wikis

Each implementation has its own structured plain-text wiki with native syntax,
examples and API references:

* PHP: `php/php_8.5/Doku/README.txt`
* Java: `java/java-utilities/Doku/README.txt`
* .NET: `dotnet/net10/RSEV.Utilities/Doku/README.txt`
* Python: `python/Doku/README.txt`
* Rust: `rust/rsev-utilities/Doku/README.txt`

## Goals

* Consistent utility layer across platforms
* No hidden magic
* Minimal dependencies
* Plugin-ready architecture
* System-level clarity

## Architecture

The implementations share the same conceptual building blocks where they fit the host language:

* Core utilities
* Configuration
* Communication / SMTP / Mail
* Logging
* Plugin system
* Runtime support
* Controllers and command handling
* Eventing and dependency injection
* Managed process lifecycles and health checks
* Dependency-free runtime API hosts and clients
* Manifest-defined database adapters for MySQL, MSSQL, PostgreSQL and SQLite
* Provider-neutral connection factories and pools
* HAMAC/HMAC-SHA256 and SHA-256 crypto providers with key/session primitives
* API clients with retry and rate-limit policies
* Access-token, refresh-token, JWT and session-manager contracts
* Versioned plugin extension contracts

The implementation plan is recorded in [manifest.json](manifest.json). Its
modules are implemented directly in all five platform trees; no generated
source tree or third-party runtime dependency is introduced.

## Status

| Platform | Version | Status |
| -------- | ------- | ------ |
| Python | 3.0.0 | Active |
| Java | 3.0.0 | Active |
| .NET | 3.0.0 | Active |
| PHP 8.5 | 3.0.0 | Active |
| Rust | 3.0.0 | Active |

## Version 3.0.0

All platform implementations now expose the common 2.0 building blocks where
they fit the host language:

* Typed event registries with handler/listener subscriptions
* Runtime subprocess controllers with lifecycle and health checks
* Process adapters for shell, Python, Node.js, .NET, and arbitrary executables
* Lightweight runtime API hosts, endpoint controllers, request builders, and clients

Platform-specific details are documented in each implementation's README and
CHANGELOG file.

## Roadmap

* Maintain feature parity across platforms
* Continue namespace and structure alignment between implementations
* Expand tooling and examples

## Philosophy

This is not a shared runtime.

It is a **shared design language for utilities**.



