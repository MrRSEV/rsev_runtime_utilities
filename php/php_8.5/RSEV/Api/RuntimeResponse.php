<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeResponse
{
    public function __construct(public readonly int $statusCode, public readonly string $body = '', public readonly array $headers = [], public readonly ?string $errorMessage = null) {}
    public function isError(): bool { return $this->statusCode >= 400; }
    public function getBodyAsText(): string { return $this->body; }
}
