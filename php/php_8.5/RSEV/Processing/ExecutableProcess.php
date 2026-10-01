<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

class ExecutableProcess extends RuntimeSubprocessBase
{
    public function __construct(string $name, protected readonly string $program, protected readonly array $programArguments = [])
    {
        parent::__construct($name);
    }

    public function executable(): string { return $this->program; }
    public function arguments(): array { return $this->programArguments; }
}
