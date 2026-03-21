# RSEV Utilities

A unified utility framework for object-oriented platforms.

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

Each platform has its own implementation, documentation, and release cycle.

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

## Status

| Platform | Status |
| -------- | ------ |
| Python | Active |
| Java | Active |
| .NET | Active |
| PHP 8.5 | Active |

## Roadmap

* Maintain feature parity across platforms
* Continue namespace and structure alignment between implementations
* Expand tooling and examples

## Philosophy

This is not a shared runtime.

It is a **shared design language for utilities**.



