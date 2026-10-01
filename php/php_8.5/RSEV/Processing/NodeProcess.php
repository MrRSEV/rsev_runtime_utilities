<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

final class NodeProcess extends ExecutableProcess
{
    public function __construct(string $name, string $script, array $arguments = [])
    {
        parent::__construct($name, 'node', [$script, ...$arguments]);
    }
}
