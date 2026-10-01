<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

final class PythonProcess extends ExecutableProcess
{
    public function __construct(string $name, string $script, array $arguments = [])
    {
        parent::__construct($name, getenv('PYTHON') ?: 'python', [$script, ...$arguments]);
    }
}
