<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

interface IRuntimeEndpoint
{
    public function route(): string;
    public function handle(EndpointRequest $request): EndpointResponse;
}
