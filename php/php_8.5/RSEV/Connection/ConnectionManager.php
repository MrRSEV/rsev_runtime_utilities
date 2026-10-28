<?php

namespace RSEV\Connection;

defined('RSEV_EXEC') or die;

final class ConnectionManager implements IConnectionManager
{
    private readonly ConnectionPool $pool;
    public function __construct(private readonly ConnectionFactory $factory, ConnectionConfiguration $configuration) { $this->pool = new ConnectionPool($configuration->maximumPoolSize); }
    public function open(ConnectionConfiguration $configuration): \PDO { return $this->pool->rent(fn () => $this->factory->create($configuration)); }
    public function giveBack(\PDO $connection): void { $this->pool->giveBack($connection); }
    public function clear(): void { $this->pool->clear(); }
}
