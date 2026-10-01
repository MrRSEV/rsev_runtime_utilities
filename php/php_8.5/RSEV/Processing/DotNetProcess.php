<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

final class DotNetProcess extends ExecutableProcess
{
    public function __construct(string $name, string $assembly, array $arguments = [])
    {
        parent::__construct($name, 'dotnet', [$assembly, ...$arguments]);
    }
}
