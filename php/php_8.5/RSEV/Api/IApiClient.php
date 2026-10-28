<?php

namespace RSEV\Api;

defined('RSEV_EXEC') or die;

interface IApiClient
{
    public function send(ApiRequest $request): ApiResponse;
}
