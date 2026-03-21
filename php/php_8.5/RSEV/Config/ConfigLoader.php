<?php

namespace RSEV\Config;

defined('RSEV_EXEC') or die;

class ConfigLoader
{
    protected array $config = [];

    public function load(string $file): void
    {
        if (!file_exists($file)) {
            return;
        }

        $this->config = include $file;
    }

    public function get(string $key, $default = null)
    {
        return $this->config[$key] ?? $default;
    }

    public function all(): array
    {
        return $this->config;
    }
}
