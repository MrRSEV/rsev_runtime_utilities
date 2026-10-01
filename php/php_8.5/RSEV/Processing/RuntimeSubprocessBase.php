<?php

namespace RSEV\Processing;

defined('RSEV_EXEC') or die;

abstract class RuntimeSubprocessBase implements IRuntimeSubprocess
{
    protected mixed $process = null;
    protected array $pipes = [];
    protected string $state = 'created';
    protected ?int $processExitCode = null;
    protected array $processProperties = [];
    protected array $stdoutHandlers = [];
    protected array $stderrHandlers = [];

    public function __construct(protected readonly string $processName)
    {
        $this->processIdValue = bin2hex(random_bytes(6));
    }

    private string $processIdValue;
    public function processId(): string { return $this->processIdValue; }
    public function name(): string { return $this->processName; }
    public function state(): string { return $this->state; }
    public function exitCode(): ?int { return $this->processExitCode; }
    public function properties(): array { return $this->processProperties; }
    abstract public function executable(): string;
    abstract public function arguments(): array;

    public function start(): bool
    {
        if ($this->state === 'running' && is_resource($this->process)) {
            return true;
        }
        $this->state = 'starting';
        $descriptor = [
            0 => ['pipe', 'r'],
            1 => ['pipe', 'w'],
            2 => ['pipe', 'w'],
        ];
        $this->process = proc_open($this->commandLine(), $descriptor, $this->pipes);
        if (!is_resource($this->process)) {
            $this->state = 'faulted';
            return false;
        }
        foreach ([$this->pipes[1] ?? null, $this->pipes[2] ?? null] as $pipe) {
            if (is_resource($pipe)) stream_set_blocking($pipe, false);
        }
        $this->state = 'running';
        return true;
    }

    public function stop(): bool
    {
        if (!is_resource($this->process)) {
            $this->state = 'stopped';
            return true;
        }
        $this->state = 'stopping';
        proc_terminate($this->process);
        $this->tick();
        $this->processExitCode = proc_close($this->process);
        $this->process = null;
        $this->state = 'stopped';
        return true;
    }

    public function restart(): bool
    {
        $this->stop();
        return $this->start();
    }

    public function checkHealth(): ?bool
    {
        return is_resource($this->process) && ($this->state === 'running');
    }

    public function onStdOut(callable $handler): void { $this->stdoutHandlers[] = $handler; }
    public function onStdErr(callable $handler): void { $this->stderrHandlers[] = $handler; }

    public function tick(): void
    {
        foreach ([[$this->pipes[1] ?? null, $this->stdoutHandlers], [$this->pipes[2] ?? null, $this->stderrHandlers]] as [$pipe, $handlers]) {
            if (!is_resource($pipe)) continue;
            $output = stream_get_contents($pipe);
            if ($output === false || $output === '') continue;
            foreach (preg_split('/\R/', trim($output)) as $line) {
                if ($line === '') continue;
                foreach ($handlers as $handler) $handler($this, $line);
            }
        }
        if (is_resource($this->process)) {
            $status = proc_get_status($this->process);
            if (!$status['running'] && $this->state === 'running') {
                $this->processExitCode = $status['exitcode'];
                $this->state = $status['exitcode'] === 0 ? 'stopped' : 'faulted';
            }
        }
    }

    protected function commandLine(): string
    {
        return implode(' ', array_map('escapeshellarg', [$this->executable(), ...$this->arguments()]));
    }
}
