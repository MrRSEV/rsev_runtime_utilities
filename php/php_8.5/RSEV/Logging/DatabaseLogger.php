<?php

namespace RSEV\Logging;

use RSEV\Communication\DBController;

defined('RSEV_EXEC') or die;

class DatabaseLogger
{
    protected DBController $db;

    public function __construct(DBController $db)
    {
        $this->db = $db;
    }

    public function log(string $message): void
    {
        $this->db->execute(
            "INSERT INTO logs (message, created_at) VALUES (?, NOW())",
            [$message]
        );
    }
}
