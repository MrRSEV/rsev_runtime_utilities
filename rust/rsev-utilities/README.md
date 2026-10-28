# RSEV.Utilities for Rust

This crate mirrors the architecture of `dotnet/net10/RSEV.Utilities` while
using Rust-native semantics:

| .NET area | Rust module |
| --- | --- |
| `Runtime`, lifecycle, shutdown | `runtime` |
| `Logging` | `logging` |
| `Configuration` | `configuration` |
| `Events` | `events` |
| `Messaging` | `messaging` |
| `Commands`, `Controllers` | `commands`, `controllers` |
| `Plugins`, `Loading` | `plugins`, `loading` |
| `Processes` | `processes` |
| `ApiBuilder` | `api_builder` |
| `RuntimeClient`, TCP/TLS options | `runtime_client`, `networking` |
| `Communication` | `communication` |
| `Database` and `Connection` | `database`, `connection` |
| `Crypto` and `Key Management` | `crypto` |
| `Session` and JWT models | `session` |
| `API tools and policies` | `api_tools` |

The library intentionally has no mandatory third-party dependencies. HTTP/1.1
parsing, TCP transport and process supervision are implemented with the
standard library. The TLS option model is present, but actual TLS stream
wrapping is left to an application-selected backend (for example `rustls`),
which is the Rust equivalent of keeping the .NET TLS layer reusable.

The manifest modules are implemented with `std` only: database provider
contracts, connection pooling, SHA-256/HMAC crypto, API policies, JWT/session
models and versioned generator plugin contracts are available without a Cargo
third-party dependency.

## Usage

```rust
use rsev_utilities::{BaseRuntimeContext, RuntimeContext};

let context = BaseRuntimeContext::default();
context.set("environment".into(), "development".into());
assert_eq!(context.get("environment").as_deref(), Some("development"));
```

Build from this directory with `cargo test --workspace`.

## Module overview

* `database` — MySQL, SQL Server, PostgreSQL and SQLite adapter contracts
* `connection` — configuration, factory, pool and connection manager
* `crypto` — HAMAC/HMAC-SHA256, SHA-256 and key management
* `api_tools` — API client contracts, request/response models, retry and rate
  limit policies
* `session` — access/refresh tokens, JWT provider and session manager
* `plugins` — versioned plugin loading and registry integration

Example:

```rust
use rsev_utilities::crypto::{CryptoProvider, HamacProvider};

let crypto = HamacProvider::new(b"a-long-secret-key").unwrap();
let signature = crypto.compute(b"payload");
assert!(crypto.verify(b"payload", &signature));
```

## Documentation

* [Platform Wiki](Doku/README.txt)
* [API-Referenz](Doku/API-Referenz.txt)
* [Repository Changelog](../../CHANGELOG.md)

