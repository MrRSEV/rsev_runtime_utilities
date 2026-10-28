<?php

namespace RSEV\Crypto;

defined('RSEV_EXEC') or die;

interface ICryptoProvider
{
    public function algorithm(): string;
    public function compute(string $data): string;
    public function verify(string $data, string $signature): bool;
}
