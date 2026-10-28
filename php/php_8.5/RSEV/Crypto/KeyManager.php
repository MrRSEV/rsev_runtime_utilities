<?php

namespace RSEV\Crypto;

defined('RSEV_EXEC') or die;

final class KeyManager
{
    public function generate(int $size = 32): string { if ($size < 16) throw new \InvalidArgumentException('size must be at least 16'); return random_bytes($size); }
}
