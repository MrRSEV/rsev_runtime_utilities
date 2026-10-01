<?php

namespace RSEV\Core;

defined('RSEV_EXEC') or die;

use RSEV\Container\Container;
use RSEV\Api\RuntimeApiHost;
use RSEV\Processing\RuntimeProcessController;

class Application
{
    protected Container $container;
    protected RuntimeProcessController $processController;
    protected RuntimeApiHost $apiHost;

    public function __construct()
    {
        $this->container = new Container();
        $this->processController = new RuntimeProcessController();
        $this->apiHost = new RuntimeApiHost();
    }

    public function container(): Container
    {
        return $this->container;
    }

    public function processController(): RuntimeProcessController
    {
        return $this->processController;
    }

    public function apiHost(): RuntimeApiHost
    {
        return $this->apiHost;
    }

    public function run(callable $callback): void
    {
        $callback($this);
    }
}
