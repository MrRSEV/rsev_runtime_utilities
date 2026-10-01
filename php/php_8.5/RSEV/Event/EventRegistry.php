<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

class EventRegistry
{
    protected array $handlers = [];
    protected array $listeners = [];

    public function __construct(public readonly string $eventName)
    {
        if ($eventName === '') {
            throw new \InvalidArgumentException('Event name must not be empty');
        }
    }

    public function subscribe(callable|EventHandlerInterface|EventListenerInterface $component, ?bool $listener = null): void
    {
        $listener ??= !is_object($component) || $component instanceof EventListenerInterface;
        $target = $listener ? 'listeners' : 'handlers';
        if (!in_array($component, $this->{$target}, true)) {
            $this->{$target}[] = $component;
        }
    }

    public function subscribeHandler(callable|EventHandlerInterface $handler): void
    {
        $this->subscribe($handler, false);
    }

    public function subscribeListener(callable|EventListenerInterface $listener): void
    {
        $this->subscribe($listener, true);
    }

    public function unsubscribe(mixed $component): void
    {
        $this->handlers = array_values(array_filter($this->handlers, fn ($value) => $value !== $component));
        $this->listeners = array_values(array_filter($this->listeners, fn ($value) => $value !== $component));
    }

    public function handlers(): array { return $this->handlers; }
    public function listeners(): array { return $this->listeners; }

    public function publish(mixed $sender, mixed $event): void
    {
        foreach (array_merge($this->handlers, $this->listeners) as $component) {
            try {
                if ($component instanceof EventHandlerInterface) {
                    $component->handle($sender, $event);
                } elseif ($component instanceof EventListenerInterface) {
                    $component->listen($sender, $event);
                } else {
                    $component($event);
                }
            } catch (\Throwable) {
                // A broken subscriber must not stop the remaining subscribers.
            }
        }
    }

    public function publishAsync(mixed $sender, mixed $event): void
    {
        // PHP has no portable userland async primitive; publishing remains non-blocking per subscriber.
        $this->publish($sender, $event);
    }
}
