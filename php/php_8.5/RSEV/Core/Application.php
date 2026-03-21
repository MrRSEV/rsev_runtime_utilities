<?php

namespace RSEV\Core;

defined('RSEV_EXEC') or die;

use RSEV\Container\Container;

class Application
{
    protected Container $container;

    public function __construct()
    {
        $this->container = new Container();
    }

    public function container(): Container
    {
        return $this->container;
    }

    public function run(callable $callback): void
    {
        $callback($this);
    }
}
