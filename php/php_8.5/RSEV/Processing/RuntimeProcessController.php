<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

class RuntimeProcessController
{
    /** @var array<string, IRuntimeSubprocess> */
    protected array $processes = [];

    public function register(IRuntimeSubprocess $process): bool
    {
        if (isset($this->processes[$process->processId()])) return false;
        $this->processes[$process->processId()] = $process;
        return true;
    }

    public function registerRange(iterable $processes): int
    {
        $count = 0;
        foreach ($processes as $process) if ($this->register($process)) $count++;
        return $count;
    }

    public function exists(string $processId): bool { return isset($this->processes[$processId]); }
    public function get(string $processId): ?IRuntimeSubprocess { return $this->processes[$processId] ?? null; }
    public function getAll(): array { return array_values($this->processes); }
    public function findByName(string $name): array { return array_values(array_filter($this->processes, fn ($process) => strcasecmp($process->name(), $name) === 0)); }
    public function findByState(string $state): array { return array_values(array_filter($this->processes, fn ($process) => $process->state() === $state)); }
    public function unregister(string $processId): bool { $exists = isset($this->processes[$processId]); unset($this->processes[$processId]); return $exists; }
    public function start(string $processId): bool { return $this->get($processId)?->start() ?? false; }
    public function stop(string $processId): bool { return $this->get($processId)?->stop() ?? false; }
    public function startAll(): int { $count = 0; foreach ($this->processes as $process) if ($process->start()) $count++; return $count; }
    public function stopAll(): int { $count = 0; foreach ($this->processes as $process) if ($process->stop()) $count++; return $count; }
    public function checkAllHealth(): array { $result = []; foreach ($this->processes as $id => $process) { $process->tick(); $result[$id] = $process->checkHealth(); } return $result; }
}
