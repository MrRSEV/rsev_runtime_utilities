<?php

namespace RSEV\Collection;

defined('RSEV_EXEC') or die;

class ArrayList implements \IteratorAggregate, \Countable
{
    protected array $items = [];

    public function add($item): void
    {
        $this->items[] = $item;
    }

    public function get(int $index)
    {
        return $this->items[$index] ?? null;
    }

    public function remove(int $index): void
    {
        unset($this->items[$index]);
        $this->items = array_values($this->items);
    }

    public function map(callable $fn): self
    {
        $new = new self();
        foreach ($this->items as $item) {
            $new->add($fn($item));
        }
        return $new;
    }

    public function filter(callable $fn): self
    {
        $new = new self();
        foreach ($this->items as $item) {
            if ($fn($item)) {
                $new->add($item);
            }
        }
        return $new;
    }

    public function toArray(): array
    {
        return $this->items;
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
