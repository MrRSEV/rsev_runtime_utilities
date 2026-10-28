# RSEV Utilities Architecture

The five platform implementations share the module boundaries defined in
[`manifest.json`](manifest.json): Database, Connection, Crypto, API and Session.
Each platform uses its own standard-library abstractions and keeps provider
drivers at the application boundary.

The dependency direction is:

```text
API / Session -> Crypto / Connection -> Database adapter contract
                         \\-> Plugin contract and platform runtime context
```

No third-party runtime dependency is introduced by these modules. HTTP clients,
retry policies, rate limits, token models and plugin registries are injectable
and can therefore be replaced by applications without changing the contracts.
