<?php

namespace RSEV\Connection;

defined('RSEV_EXEC') or die;

final readonly class ConnectionConfiguration
{
    public function __construct(
        public string $provider,
        public string $connectionString,
        public string $username = '',
        public string $password = '',
        public int $minimumPoolSize = 1,
        public int $maximumPoolSize = 10,
    ) {
        if ($provider === '' || $connectionString === '') throw new \InvalidArgumentException('provider and connectionString are required');
        if ($minimumPoolSize < 0 || $maximumPoolSize < 1 || $minimumPoolSize > $maximumPoolSize) throw new \InvalidArgumentException('invalid pool bounds');
    }

    public function dsn(): string { return $this->connectionString; }
}
