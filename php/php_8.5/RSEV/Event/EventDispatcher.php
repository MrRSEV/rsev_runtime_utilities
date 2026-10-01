<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

class EventDispatcher
{
    protected array $listeners = [];

    public function listen(string $event, callable $listener): void
    {
        $this->listeners[$event][] = $listener;
    }

    public function dispatch(string $event, $payload = null): void
    {
        foreach ($this->listeners[$event] ?? [] as $listener) {
            $listener($payload);
        }
    }

    public function forget(string $event, ?callable $listener = null): void
    {
        if ($listener === null) {
            unset($this->listeners[$event]);
            return;
        }
        $this->listeners[$event] = array_values(array_filter(
            $this->listeners[$event] ?? [],
            static fn ($registered) => $registered !== $listener,
        ));
    }
}
