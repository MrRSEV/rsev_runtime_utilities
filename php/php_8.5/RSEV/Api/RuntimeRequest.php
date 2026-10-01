<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeRequest
{
    public function __construct(
        public readonly string $route,
        public readonly mixed $payload = null,
        public readonly array $headers = [],
        public readonly ?string $sessionId = null,
    ) {
    }
}
