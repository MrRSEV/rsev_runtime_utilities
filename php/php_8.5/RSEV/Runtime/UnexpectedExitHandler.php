<?php

namespace RSEV\Runtime;

use RSEV\Processing\ProcessManager;

defined('RSEV_EXEC') or die;

class UnexpectedExitHandler
{
    protected ProcessManager $pm;

    public function __construct(ProcessManager $pm)
    {
        $this->pm = $pm;
    }

    public function register(): void
    {
        register_shutdown_function([$this, 'handle']);
    }

    public function handle(): void
    {
        $error = error_get_last();

        if ($error !== null) {
            $this->pm->killAll();
        }
    }
}
