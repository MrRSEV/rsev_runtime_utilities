<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

interface IAccessTokenProvider
{
    public function create(string $subject, int $lifetimeSeconds): AccessToken;
    public function validate(AccessToken $token): bool;
}
