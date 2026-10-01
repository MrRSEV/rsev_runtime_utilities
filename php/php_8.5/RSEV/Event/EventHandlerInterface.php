<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

interface EventHandlerInterface
{
    public function handle(mixed $sender, mixed $event): void;
}
