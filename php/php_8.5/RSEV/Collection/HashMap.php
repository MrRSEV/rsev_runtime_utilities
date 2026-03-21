<?php

namespace RSEV\Collection;

defined('RSEV_EXEC') or die;

class HashMap implements \IteratorAggregate, \Countable
{
    protected array $items = [];

    public function put($key, $value): void
    {
        $this->items[$key] = $value;
    }

    public function get($key, $default = null)
    {
        return $this->items[$key] ?? $default;
    }

    public function containsKey($key): bool
    {
        return array_key_exists($key, $this->items);
    }

    public function remove($key): void
    {
        unset($this->items[$key]);
    }

    public function keys(): array
    {
        return array_keys($this->items);
    }

    public function values(): array
    {
        return array_values($this->items);
    }

    public function getIterator(): \Traversable
    {
        return new \ArrayIterator($this->items);
    }

    public function count(): int
    {
        return count($this->items);
    }
}
