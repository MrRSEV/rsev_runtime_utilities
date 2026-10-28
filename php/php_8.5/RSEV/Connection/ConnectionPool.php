<?php

namespace RSEV\Connection;

defined('RSEV_EXEC') or die;

final class ConnectionPool
{
    /** @var list<\PDO> */
    private array $connections = [];
    public function __construct(private readonly int $maximumSize = 10) { if ($maximumSize < 1) throw new \InvalidArgumentException('maximumSize must be positive'); }
    public function rent(callable $factory): \PDO { return array_pop($this->connections) ?? $factory(); }
    public function giveBack(\PDO $connection): bool { if (count($this->connections) >= $this->maximumSize) return false; $this->connections[] = $connection; return true; }
    public function clear(): void { $this->connections = []; }
}
