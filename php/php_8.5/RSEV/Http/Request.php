<?php

namespace RSEV\Http;

defined('RSEV_EXEC') or die;

class Request
{
    public function method(): string
    {
        return $_SERVER['REQUEST_METHOD'] ?? 'GET';
    }

    public function uri(): string
    {
        return strtok($_SERVER['REQUEST_URI'] ?? '/', '?');
    }

    public function input(string $key, $default = null)
    {
        return $_REQUEST[$key] ?? $default;
    }
}
