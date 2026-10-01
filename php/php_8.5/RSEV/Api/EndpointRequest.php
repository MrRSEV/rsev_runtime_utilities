<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class EndpointRequest
{
    public function __construct(
        public readonly string $method,
        public readonly string $route,
        public readonly array $headers = [],
        public readonly string $body = '',
    ) {
    }

    public function payload(): mixed
    {
        if ($this->body === '') return null;
        $decoded = json_decode($this->body, true);
        return json_last_error() === JSON_ERROR_NONE ? $decoded : $this->body;
    }
}
