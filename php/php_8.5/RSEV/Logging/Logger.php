<?php

namespace RSEV\Logging;

defined('RSEV_EXEC') or die;

class Logger
{
    protected string $path;
    protected string $defaultFile;

    public function __construct(?string $path = null, string $defaultFile = 'latest.log')
    {
        $this->path = $path ?? __DIR__ . '/../../logs';
        $this->defaultFile = $defaultFile;

        if (!is_dir($this->path)) {
            mkdir($this->path, 0777, true);
        }
    }

    public function info(string $message): void
    {
        $this->write('INFO', $message);
    }

    public function error(string $message): void
    {
        $this->write('ERROR', $message);
    }

    protected function write(string $level, string $message): void
    {
        $this->log("{$level}: {$message}", $this->defaultFile);
    }

    public function log(string $message, string $file = 'latest.log'): void
    {
        file_put_contents(
            $this->path . '/' . $file,
            '[' . date('Y-m-d H:i:s') . '] ' . $message . PHP_EOL,
            FILE_APPEND
        );
    }
}
