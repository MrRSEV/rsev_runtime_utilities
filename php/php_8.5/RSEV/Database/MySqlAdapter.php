<?php

namespace RSEV\Database;

defined('RSEV_EXEC') or die;

final class MySqlAdapter implements IDatabaseAdapter
{
    public function provider(): string { return 'mysql'; }
    public function createConnection(\RSEV\Connection\ConnectionConfiguration $configuration): \PDO
    {
        return new \PDO($configuration->dsn(), $configuration->username, $configuration->password, [\PDO::ATTR_ERRMODE => \PDO::ERRMODE_EXCEPTION]);
    }
}
