<?php

namespace RSEV\Container;

defined('RSEV_EXEC') or die;

class Container
{
    protected array $bindings = [];
    protected array $instances = [];

    public function bind(string $key, callable $resolver): void
    {
        $this->bindings[$key] = $resolver;
    }

    public function singleton(string $key, callable $resolver): void
    {
        $this->bindings[$key] = function ($c) use ($resolver, $key) {
            if (!isset($this->instances[$key])) {
                $this->instances[$key] = $resolver($c);
            }
            return $this->instances[$key];
        };
    }

    public function get(string $key)
    {
        if (!isset($this->bindings[$key])) {
            throw new \Exception("Service {$key} not found");
        }

        return $this->bindings[$key]($this);
    }
}
