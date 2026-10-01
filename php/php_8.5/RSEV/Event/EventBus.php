<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

class EventBus
{
    protected array $registries = [];

    public function registerEventRegistry(EventRegistry $registry, string $eventType = Event::class): void
    {
        $this->registries[$this->key($eventType, $registry->eventName)] = $registry;
    }

    public function getRegistry(string $eventName, string $eventType = Event::class): ?EventRegistry
    {
        return $this->registries[$this->key($eventType, $eventName)] ?? null;
    }

    public function hasRegistry(string $eventName, string $eventType = Event::class): bool
    {
        return $this->getRegistry($eventName, $eventType) !== null;
    }

    public function getAllEventNames(): array
    {
        return array_values(array_unique(array_map(fn (EventRegistry $registry) => $registry->eventName, $this->registries)));
    }

    public function publish(string|Event $event, mixed $payload = null, mixed $sender = null): void
    {
        $eventName = $event instanceof Event ? $event->name : $event;
        $registry = $this->getRegistry($eventName, $event instanceof Event ? $event::class : Event::class)
            ?? $this->getRegistry($eventName);
        $registry?->publish($sender ?? $this, $payload ?? $event);
    }

    protected function key(string $eventType, string $eventName): string
    {
        return $eventType . '::' . $eventName;
    }
}
