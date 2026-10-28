<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

final readonly class AccessToken
{
    public function __construct(public string $value, public \DateTimeImmutable $expiresAt) {}
    public function isExpired(): bool { return $this->expiresAt <= new \DateTimeImmutable('now', new \DateTimeZone('UTC')); }
}
