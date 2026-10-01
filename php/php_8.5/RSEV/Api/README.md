# Runtime API

The Runtime API is part of the **2.0.0** release.

The API host is intentionally small and dependency-free. Register `IRuntimeEndpoint`
implementations, call `start()`, and service connections by calling `tick()` from the
owning application's loop. `RuntimeApiClient` uses the same HTTP/1.1-style wire format.

```php
require 'RSEV/Init.php';

use RSEV\Api\BaseRuntimeEndpoint;
use RSEV\Api\EndpointResponse;
use RSEV\Api\RuntimeApiHost;

$host = new RuntimeApiHost('127.0.0.1', 8080);
$host->register(new class('/health') extends BaseRuntimeEndpoint {
    protected function onHandle(\RSEV\Api\EndpointRequest $request): EndpointResponse
    {
        return EndpointResponse::fromText(200, 'ok');
    }
});
$host->start();
```

The matching client is `RSEV\Api\RuntimeApiClient`; request payloads can be
constructed with `RuntimeRequestBuilder`, and error responses expose `isError()`.
