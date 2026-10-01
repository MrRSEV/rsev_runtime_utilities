<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

final class ShellProcess extends ExecutableProcess
{
    public function __construct(string $name, string $command)
    {
        parent::__construct($name, PHP_OS_FAMILY === 'Windows' ? 'cmd.exe' : '/bin/sh', PHP_OS_FAMILY === 'Windows' ? ['/c', $command] : ['-c', $command]);
    }
}
