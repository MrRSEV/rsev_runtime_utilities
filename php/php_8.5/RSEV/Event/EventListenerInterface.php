<?php

namespace RSEV\Event;

defined('RSEV_EXEC') or die;

interface EventListenerInterface
{
    public function listen(mixed $sender, mixed $event): void;
}
