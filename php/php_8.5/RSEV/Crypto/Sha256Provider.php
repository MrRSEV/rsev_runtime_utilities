<?php

namespace RSEV\Crypto;

defined('RSEV_EXEC') or die;

final class Sha256Provider implements ICryptoProvider
{
    public function algorithm(): string { return 'SHA-256'; }
    public function compute(string $data): string { return hash('sha256', $data, true); }
    public function verify(string $data, string $signature): bool { return hash_equals($this->compute($data), $signature); }
}
