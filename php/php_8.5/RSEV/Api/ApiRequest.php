<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

final readonly class ApiRequest
{
    public function __construct(public string $method, public string $url, public array $headers = [], public string $body = '') {}
}
