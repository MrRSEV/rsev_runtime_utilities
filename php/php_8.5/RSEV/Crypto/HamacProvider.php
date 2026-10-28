<?php

namespace RSEV\Crypto;

defined('RSEV_EXEC') or die;

final readonly class HamacProvider implements ICryptoProvider
{
    public function __construct(private string $key) { if ($key === '') throw new \InvalidArgumentException('key is required'); }
    public function algorithm(): string { return 'HAMAC-SHA256'; }
    public function compute(string $data): string { return hash_hmac('sha256', $data, $this->key, true); }
    public function verify(string $data, string $signature): bool { return hash_equals($this->compute($data), $signature); }
}
