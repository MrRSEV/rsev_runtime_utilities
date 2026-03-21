<?php

namespace RSEV\Utils;

defined('RSEV_EXEC') or die;

class Arr
{
    public static function get(array $array, string $key, $default = null)
    {
        return $array[$key] ?? $default;
    }

    public static function set(array &$array, string $key, $value): void
    {
        $array[$key] = $value;
    }
}
