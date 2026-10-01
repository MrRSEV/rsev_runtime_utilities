<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeEndpointController
{
    protected array $endpoints = [];
    public function register(IRuntimeEndpoint $endpoint): bool { if (isset($this->endpoints[$endpoint->route()])) return false; $this->endpoints[$endpoint->route()] = $endpoint; return true; }
    public function unregister(string $route): bool { $exists = isset($this->endpoints[$route]); unset($this->endpoints[$route]); return $exists; }
    public function resolve(string $route): ?IRuntimeEndpoint { return $this->endpoints[$route] ?? null; }
    public function getAll(): array { return array_values($this->endpoints); }
}
