<?php

namespace RSEV\Database;

defined('RSEV_EXEC') or die;

interface IDatabaseAdapter
{
    public function provider(): string;
    public function createConnection(\RSEV\Connection\ConnectionConfiguration $configuration): \PDO;
}
