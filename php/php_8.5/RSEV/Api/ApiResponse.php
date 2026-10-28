<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

final readonly class ApiResponse
{
    public function __construct(public int $statusCode, public string $body = '', public array $headers = []) {}
    public function isSuccess(): bool { return $this->statusCode >= 200 && $this->statusCode < 300; }
}
