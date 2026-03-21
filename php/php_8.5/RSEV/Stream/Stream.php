<?php

namespace RSEV\Stream;

defined('RSEV_EXEC') or die;

class Stream
{
    protected array $data;

    private function __construct(array $data)
    {
        $this->data = $data;
    }

    public static function of(array $data): self
    {
        return new self($data);
    }

    public function map(callable $fn): self
    {
        return new self(array_map($fn, $this->data));
    }

    public function filter(callable $fn): self
    {
        return new self(array_filter($this->data, $fn));
    }

    public function forEach(callable $fn): void
    {
        foreach ($this->data as $item) {
            $fn($item);
        }
    }

    public function toArray(): array
    {
        return $this->data;
    }
}
