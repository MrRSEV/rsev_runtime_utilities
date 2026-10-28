<?php

namespace RSEV\Session;

defined('RSEV_EXEC') or die;

interface ISessionManager
{
    public function create(string $subject): SessionContext;
    public function isValid(SessionContext $context): bool;
}
