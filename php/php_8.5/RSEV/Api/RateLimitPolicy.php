<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

final class RateLimitPolicy
{
    private array $requests = [];
    public function __construct(private readonly int $requestsPerWindow = 60, private readonly int $windowSeconds = 60) { if ($requestsPerWindow < 1 || $windowSeconds < 1) throw new \InvalidArgumentException('invalid rate limit'); }
    public function tryAcquire(): bool
    {
        $cutoff = microtime(true) - $this->windowSeconds;
        $this->requests = array_values(array_filter($this->requests, static fn (float $time): bool => $time > $cutoff));
        if (count($this->requests) >= $this->requestsPerWindow) return false;
        $this->requests[] = microtime(true);
        return true;
    }
}
