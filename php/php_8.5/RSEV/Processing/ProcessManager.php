<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

class ProcessManager
{
    protected array $processes = [];

    public function start(string $command): int
    {
        $descriptors = [];
        $process = proc_open($command, $descriptors, $pipes);

        if (is_resource($process)) {
            $status = proc_get_status($process);
            $pid = $status['pid'];
            $this->processes[$pid] = $process;
            return $pid;
        }

        throw new \RuntimeException("Process failed");
    }

    public function stop(int $pid): void
    {
        if (isset($this->processes[$pid])) {
            proc_terminate($this->processes[$pid]);
            unset($this->processes[$pid]);
        }
    }

    public function killAll(): void
    {
        foreach ($this->processes as $pid => $proc) {
            proc_terminate($proc);
        }
        $this->processes = [];
    }

    public function isRunning(int $pid): bool
    {
        if (!isset($this->processes[$pid])) return false;
        $status = proc_get_status($this->processes[$pid]);
        return (bool) $status['running'];
    }
}
