<?php

namespace RSEV\Controller;

defined('RSEV_EXEC') or die;

interface ControllerInterface
{
    public function handle(array $request): mixed;
}

abstract class BaseController implements ControllerInterface
{
    public function handle(array $request): mixed
    {
        return $this->process($request);
    }

    abstract protected function process(array $request): mixed;
}
