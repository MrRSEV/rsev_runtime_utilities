<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

class RuntimeSession
{
    public function __construct(public readonly string $sessionId, public readonly ?\DateTimeImmutable $expiresAt = null, public readonly array $metadata = []) {}
    public function isExpired(): bool { return $this->expiresAt !== null && $this->expiresAt <= new \DateTimeImmutable(); }
    public function renew(): void {}
}
