<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

final readonly class RetryPolicy
{
    public function __construct(public int $maxAttempts = 3, public int $delayMilliseconds = 200) { if ($maxAttempts < 1 || $delayMilliseconds < 0) throw new \InvalidArgumentException('invalid retry policy'); }
}
