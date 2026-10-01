<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

class Event
{
    public function __construct(
        public readonly string $name,
        public readonly string $description = '',
        public readonly string $eventType = self::class,
    ) {
        if ($name === '') {
            throw new \InvalidArgumentException('Event name must not be empty');
        }
    }
}
