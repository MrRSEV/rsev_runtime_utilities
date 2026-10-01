# RSEV Utilities (Python)

Python implementation of the RSEV Utilities framework.

Current release: **2.0.0**.

## Install

```bash
pip install rsev_utilities
```

The package metadata declares Python `>=3.10` and release `2.0.0`.

## Scope

This module provides:

* Core utilities
* Config system
* SMTP client
* Logging
* Plugin system
* EventBus
* Typed event registries with synchronous/asynchronous publishing
* DI container
* Command framework

## Notes

This implementation follows the same architecture as:

* Java version
* .NET version

Canonical Python namespaces now mirror the Java-style module names:

* `rsev_utilities.command`
* `rsev_utilities.communication`
* `rsev_utilities.configuration`
* `rsev_utilities.controller`
* `rsev_utilities.logging`
* `rsev_utilities.plugin`
* `rsev_utilities.runtime`
* `rsev_utilities.events`
* `rsev_utilities.api`
* `rsev_utilities.runtime.processing`

See root README for full vision.

Runtime 2.0 also provides `RuntimeProcessController`, managed process adapters,
`RuntimeApiHost`, `RuntimeApiClient`, request/response models, and session support.

```python
from rsev_utilities import BaseRuntimeEndpoint, EndpointResponse, RuntimeApiHost

endpoint = BaseRuntimeEndpoint("/health")
endpoint.on_handle = lambda request: EndpointResponse.from_text(200, "ok")
host = RuntimeApiHost(port=8080)
host.register(endpoint)
host.start()
```
