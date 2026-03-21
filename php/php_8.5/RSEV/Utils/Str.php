<?php

namespace RSEV\Utils;

defined('RSEV_EXEC') or die;

class Str
{
    public static function contains(string $haystack, string $needle): bool
    {
        return str_contains($haystack, $needle);
    }

    public static function upper(string $string): string
    {
        return strtoupper($string);
    }

    public static function lower(string $string): string
    {
        return strtolower($string);
    }
}
