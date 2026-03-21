<?php

namespace RSEV\Utils;

defined('RSEV_EXEC') or die;

class Optional
{
    private $value;

    private function __construct($value)
    {
        $this->value = $value;
    }

    public static function of($value): self
    {
        if ($value === null) {
            throw new \InvalidArgumentException("Value cannot be null");
        }
        return new self($value);
    }

    public static function ofNullable($value): self
    {
        return new self($value);
    }

    public static function empty(): self
    {
        return new self(null);
    }

    public function isPresent(): bool
    {
        return $this->value !== null;
    }

    public function get()
    {
        if ($this->value === null) {
            throw new \RuntimeException("No value present");
        }
        return $this->value;
    }

    public function orElse($other)
    {
        return $this->value ?? $other;
    }

    public function ifPresent(callable $consumer): void
    {
        if ($this->isPresent()) {
            $consumer($this->value);
        }
    }
}
