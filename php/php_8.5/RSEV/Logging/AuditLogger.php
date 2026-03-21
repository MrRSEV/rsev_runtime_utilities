<?php

namespace RSEV\Logging;

defined('RSEV_EXEC') or die;

class AuditLogger extends Logger
{
    public function audit(string $message): void
    {
        $this->log('[AUDIT] ' . $message, 'audit.log');
    }
}
