<?php

namespace RSEV\Config;

defined('RSEV_EXEC') or die;

class Config
{
    protected static array $data = [];

    public static function set(string $key, $value): void
    {
        static::$data[$key] = $value;
    }

    public static function get(string $key, $default = null)
    {
        return static::$data[$key] ?? $default;
    }
}
