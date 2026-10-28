<?php

namespace RSEV\Connection;

defined('RSEV_EXEC') or die;

interface IConnectionManager
{
    public function open(ConnectionConfiguration $configuration): \PDO;
    public function giveBack(\PDO $connection): void;
}
