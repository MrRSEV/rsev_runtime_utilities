<?php

namespace RSEV\Connection;

defined('RSEV_EXEC') or die;

final readonly class ConnectionFactory
{
    public function __construct(private \RSEV\Database\IDatabaseAdapter $adapter) {}
    public function create(ConnectionConfiguration $configuration): \PDO { return $this->adapter->createConnection($configuration); }
}
