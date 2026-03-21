<?php

namespace RSEV\Utils;

defined('RSEV_EXEC') or die;

class MathUtil
{
    public static function clamp($value, $min, $max)
    {
        return max($min, min($max, $value));
    }

    public static function randomInt(int $min, int $max): int
    {
        return random_int($min, $max);
    }

    public static function average(array $numbers): float
    {
        return array_sum($numbers) / max(count($numbers), 1);
    }
}
