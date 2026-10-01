<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

interface IRuntimeSubprocess
{
    public function processId(): string;
    public function name(): string;
    public function executable(): string;
    public function arguments(): array;
    public function state(): string;
    public function exitCode(): ?int;
    public function properties(): array;
    public function start(): bool;
    public function stop(): bool;
    public function restart(): bool;
    public function checkHealth(): ?bool;
}
