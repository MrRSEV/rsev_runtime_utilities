<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

abstract class BaseRuntimeEndpoint implements IRuntimeEndpoint
{
    public function __construct(protected readonly string $endpointRoute)
    {
        if (!str_starts_with($endpointRoute, '/')) throw new \InvalidArgumentException("route must start with '/'");
    }

    public function route(): string { return $this->endpointRoute; }
    public function handle(EndpointRequest $request): EndpointResponse { return $this->onHandle($request); }
    abstract protected function onHandle(EndpointRequest $request): EndpointResponse;
}
